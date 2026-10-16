using EventFlow.Messaging.Kafka;
using EventFlow.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace EventFlow.Messaging.Tests;

public sealed class OutboxPublisherTests
{
    [Fact]
    public async Task Publisher_publishes_a_pending_message_and_marks_it_processed()
    {
        var producer = new FakeKafkaProducer();

        await using var provider = TestServiceProvider.Build("publish-ok", producer);

        await SeedAsync(provider, new TestEvent
        {
            MessageId = Guid.NewGuid(),
            EventId = Guid.NewGuid(),
            EventContextId = Guid.NewGuid(),
            Name = "publish-me",
            OccurredAtUtc = DateTime.UtcNow
        });

        await RunPublisherAsync(provider, producer, MaxAttempts: 3, until: db => db.OutboxMessages.Single().Status == OutboxStatus.Processed);

        using var scope = provider.CreateScope();
        var stored = scope.ServiceProvider.GetRequiredService<TestOutboxDbContext>().OutboxMessages.Single();

        Assert.Equal(OutboxStatus.Processed, stored.Status);
        Assert.NotNull(stored.ProcessedAtUtc);
        Assert.Single(producer.Published);
        Assert.Equal("test-topic", producer.Published[0].Topic);
    }

    [Fact]
    public async Task Publisher_retries_after_a_transient_publish_failure()
    {
        var producer = new FakeKafkaProducer { FailTimes = 1 };

        await using var provider = TestServiceProvider.Build("publish-retry", producer);

        await SeedAsync(provider, new TestEvent
        {
            MessageId = Guid.NewGuid(),
            EventId = Guid.NewGuid(),
            EventContextId = Guid.NewGuid(),
            Name = "retry-me",
            OccurredAtUtc = DateTime.UtcNow
        });

        await RunPublisherAsync(provider, producer, MaxAttempts: 5, until: db => db.OutboxMessages.Single().Status == OutboxStatus.Processed);

        using var scope = provider.CreateScope();
        var stored = scope.ServiceProvider.GetRequiredService<TestOutboxDbContext>().OutboxMessages.Single();

        Assert.Equal(OutboxStatus.Processed, stored.Status);
        Assert.Equal(2, stored.Attempts);
        Assert.Single(producer.Published);
    }

    [Fact]
    public async Task Publisher_marks_a_message_failed_after_the_attempt_limit()
    {
        var producer = new FakeKafkaProducer { FailTimes = int.MaxValue };

        await using var provider = TestServiceProvider.Build("publish-fail", producer);

        await SeedAsync(provider, new TestEvent
        {
            MessageId = Guid.NewGuid(),
            EventId = Guid.NewGuid(),
            EventContextId = Guid.NewGuid(),
            Name = "fail-me",
            OccurredAtUtc = DateTime.UtcNow
        });

        await RunPublisherAsync(provider, producer, MaxAttempts: 2, until: db => db.OutboxMessages.Single().Status == OutboxStatus.Failed);

        using var scope = provider.CreateScope();
        var stored = scope.ServiceProvider.GetRequiredService<TestOutboxDbContext>().OutboxMessages.Single();

        Assert.Equal(OutboxStatus.Failed, stored.Status);
        Assert.True(stored.Attempts >= 2);
        Assert.NotNull(stored.Error);
        Assert.Empty(producer.Published);
    }

    private static async Task SeedAsync(ServiceProvider provider, TestEvent message)
    {
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<IOutbox>().Enqueue("test-topic", message);

        await scope.ServiceProvider.GetRequiredService<TestOutboxDbContext>().SaveChangesAsync();
    }

    private static async Task RunPublisherAsync(
        ServiceProvider provider,
        FakeKafkaProducer producer,
        int MaxAttempts,
        Func<TestOutboxDbContext, bool> until)
    {
        var settings = new OutboxOptions
        {
            BatchSize = 10,
            MaxAttempts = MaxAttempts,
            PollIntervalSeconds = 1,
            RetryBackoffSeconds = 1,
            LockTimeoutSeconds = 60
        };

        var publisher = new OutboxPublisher<TestOutboxDbContext>(
            provider.GetRequiredService<IServiceScopeFactory>(),
            producer,
            Options.Create(settings),
            NullLogger<OutboxPublisher<TestOutboxDbContext>>.Instance);

        await publisher.StartAsync(CancellationToken.None);

        var deadline = DateTime.UtcNow.AddSeconds(15);

        while (DateTime.UtcNow < deadline)
        {
            using (var scope = provider.CreateScope())
            {
                if (until(scope.ServiceProvider.GetRequiredService<TestOutboxDbContext>()))
                {
                    break;
                }
            }

            await Task.Delay(100);
        }

        await publisher.StopAsync(CancellationToken.None);
    }
}
