using EventFlow.Messaging.Kafka;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Infrastructure.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventFlow.Operations.Infrastructure.Tests;

public sealed class KafkaConsumerTests
{
    [Fact]
    public async Task Processes_a_message_once()
    {
        var producer = new FakeKafkaProducer();
        await using var provider = BuildProvider("consumer-success", producer);
        var consumer = BuildConsumer(provider, producer, AddNotification, maxAttempts: 3);
        var options = BuildOptions(maxAttempts: 3);

        var outcome = await consumer.ProcessMessageAsync(
            "key", Serialize(NewMessage("payload")), NullLogger.Instance, options, CancellationToken.None);

        Assert.Equal(MessageOutcome.Processed, outcome);

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestOperationsDbContext>();

        Assert.Equal(1, await db.Notifications.CountAsync());
        Assert.Equal(1, await db.ProcessedMessages.CountAsync());
    }

    [Fact]
    public async Task Duplicate_delivery_does_not_repeat_side_effects()
    {
        var producer = new FakeKafkaProducer();
        await using var provider = BuildProvider("consumer-duplicate", producer);

        var handled = 0;
        var consumer = BuildConsumer(provider, producer, (message, db, ct) =>
        {
            handled++;
            return AddNotification(message, db, ct);
        }, maxAttempts: 3);

        var options = BuildOptions(maxAttempts: 3);
        var json = Serialize(NewMessage("payload"));

        var first = await consumer.ProcessMessageAsync("key", json, NullLogger.Instance, options, CancellationToken.None);
        var second = await consumer.ProcessMessageAsync("key", json, NullLogger.Instance, options, CancellationToken.None);

        Assert.Equal(MessageOutcome.Processed, first);
        Assert.Equal(MessageOutcome.Duplicate, second);
        Assert.Equal(1, handled);

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestOperationsDbContext>();

        Assert.Equal(1, await db.Notifications.CountAsync());
        Assert.Equal(1, await db.ProcessedMessages.CountAsync());
    }

    [Fact]
    public async Task Retries_after_a_database_failure()
    {
        var producer = new FakeKafkaProducer();
        await using var provider = BuildProvider("consumer-retry", producer);
        provider.GetRequiredService<FailureController>().RemainingFailures = 1;

        var consumer = BuildConsumer(provider, producer, AddNotification, maxAttempts: 3);
        var options = BuildOptions(maxAttempts: 3);

        var outcome = await consumer.ProcessMessageAsync(
            "key", Serialize(NewMessage("payload")), NullLogger.Instance, options, CancellationToken.None);

        Assert.Equal(MessageOutcome.Processed, outcome);

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestOperationsDbContext>();

        Assert.Equal(1, await db.Notifications.CountAsync());
        Assert.Equal(1, await db.ProcessedMessages.CountAsync());
    }

    [Fact]
    public async Task Dead_letters_after_repeated_database_failures()
    {
        var producer = new FakeKafkaProducer();
        await using var provider = BuildProvider("consumer-db-down", producer);
        provider.GetRequiredService<FailureController>().RemainingFailures = int.MaxValue;

        var consumer = BuildConsumer(provider, producer, AddNotification, maxAttempts: 2);
        var options = BuildOptions(maxAttempts: 2);

        var outcome = await consumer.ProcessMessageAsync(
            "key", Serialize(NewMessage("payload")), NullLogger.Instance, options, CancellationToken.None);

        Assert.Equal(MessageOutcome.DeadLettered, outcome);
        Assert.Contains(producer.Published, x => x.Topic == "test-topic.dlq");

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestOperationsDbContext>();

        Assert.Equal(0, await db.ProcessedMessages.CountAsync());
    }

    [Fact]
    public async Task Dead_letters_a_poison_message()
    {
        var producer = new FakeKafkaProducer();
        await using var provider = BuildProvider("consumer-poison", producer);
        var consumer = BuildConsumer(provider, producer, AddNotification, maxAttempts: 3);
        var options = BuildOptions(maxAttempts: 3);

        var outcome = await consumer.ProcessMessageAsync(
            "key", "{ this is not valid json", NullLogger.Instance, options, CancellationToken.None);

        Assert.Equal(MessageOutcome.DeadLettered, outcome);
        Assert.Contains(producer.Published, x => x.Topic == "test-topic.dlq");
    }

    [Fact]
    public async Task Keeps_the_message_when_the_dead_letter_publish_fails()
    {
        var producer = new FakeKafkaProducer { FailTimes = int.MaxValue };
        await using var provider = BuildProvider("consumer-dlq-down", producer);
        var consumer = BuildConsumer(provider, producer, AddNotification, maxAttempts: 3);
        var options = BuildOptions(maxAttempts: 3);

        await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.ProcessMessageAsync(
            "key", "{ this is not valid json", NullLogger.Instance, options, CancellationToken.None));

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestOperationsDbContext>();

        Assert.Equal(0, await db.ProcessedMessages.CountAsync());
        Assert.Empty(producer.Published);
    }

    private static TestIntegrationEvent NewMessage(string payload) => new()
    {
        MessageId = Guid.NewGuid(),
        EventId = Guid.NewGuid(),
        EventContextId = Guid.NewGuid(),
        Payload = payload,
        OccurredAtUtc = DateTime.UtcNow
    };

    private static string Serialize(TestIntegrationEvent message) => JsonSerializer.Serialize(message);

    private static Task AddNotification(TestIntegrationEvent message, IOperationsDbContext db, CancellationToken cancellationToken)
    {
        db.Notifications.Add(new Notification(
            message.EventId,
            null,
            "someone@example.com",
            "Subject",
            message.Payload,
            DateTime.UtcNow));

        return Task.CompletedTask;
    }

    private static KafkaOptions BuildOptions(int maxAttempts) => new()
    {
        ConsumerMaxAttempts = maxAttempts,
        ConsumerRetryBackoffSeconds = 0,
        DeadLetterSuffix = ".dlq"
    };

    private static ServiceProvider BuildProvider(string databaseName, FakeKafkaProducer producer)
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSingleton<FailureController>();
        services.AddSingleton<IKafkaProducer>(producer);

        services.AddScoped(provider => new TestOperationsDbContext(
            new DbContextOptionsBuilder<TestOperationsDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options,
            provider.GetRequiredService<FailureController>()));

        services.AddScoped<IOperationsDbContext>(provider =>
            provider.GetRequiredService<TestOperationsDbContext>());

        return services.BuildServiceProvider();
    }

    private static TestConsumer BuildConsumer(
        ServiceProvider provider,
        FakeKafkaProducer producer,
        Func<TestIntegrationEvent, IOperationsDbContext, CancellationToken, Task> handler,
        int maxAttempts)
    {
        return new TestConsumer(
            provider.GetRequiredService<IServiceScopeFactory>(),
            Options.Create(BuildOptions(maxAttempts)),
            producer,
            NullLoggerFactory.Instance,
            handler);
    }
}
