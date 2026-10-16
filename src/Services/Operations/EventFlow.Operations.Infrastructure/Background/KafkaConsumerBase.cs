using System.Text.Json;
using Confluent.Kafka;
using EventFlow.Messaging.Events;
using EventFlow.Messaging.Kafka;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventFlow.Operations.Infrastructure.Background;

// Shared Kafka consumer loop: at-least-once processing with an idempotency
// check, per-message retries and a dead-letter topic for poison messages.
public abstract class KafkaConsumerBase<TEvent>(
    IServiceScopeFactory scopeFactory,
    IOptions<KafkaOptions> kafkaOptions,
    IKafkaProducer producer,
    ILoggerFactory loggerFactory) : BackgroundService
    where TEvent : IntegrationEvent
{
    protected abstract string Topic { get; }

    protected abstract string GroupId { get; }

    protected abstract string ConsumerName { get; }

    protected abstract Task HandleMessageAsync(
        TEvent message,
        IOperationsDbContext db,
        CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var logger = loggerFactory.CreateLogger(GetType());
        var options = kafkaOptions.Value;

        var config = new ConsumerConfig
        {
            BootstrapServers = options.BootstrapServers,
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
                logger.LogError("Kafka consumer {Consumer} error: {Reason}", ConsumerName, error.Reason))
            .SetPartitionsAssignedHandler((_, partitions) =>
                logger.LogInformation(
                    "Kafka consumer {Consumer} assigned: {Partitions}",
                    ConsumerName,
                    string.Join(", ", partitions)))
            .SetPartitionsRevokedHandler((_, partitions) =>
                logger.LogWarning(
                    "Kafka consumer {Consumer} revoked: {Partitions}",
                    ConsumerName,
                    string.Join(", ", partitions)))
            .Build();

        consumer.Subscribe(Topic);

        logger.LogInformation("Kafka consumer {Consumer} started on {Topic}.", ConsumerName, Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result;

                try
                {
                    result = consumer.Consume(stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                if (result?.Message?.Value is null)
                {
                    logger.LogWarning("Consumer {Consumer} received a message with an empty value.", ConsumerName);
                    continue;
                }

                await ProcessAsync(consumer, result, logger, options, stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Kafka consumer {Consumer} stopped.", ConsumerName);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Kafka consumer {Consumer} stopped because of an unexpected error.", ConsumerName);
        }
        finally
        {
            consumer.Close();

            logger.LogInformation("Kafka consumer {Consumer} closed.", ConsumerName);
        }
    }

    private async Task ProcessAsync(
        IConsumer<string, string> consumer,
        ConsumeResult<string, string> result,
        ILogger logger,
        KafkaOptions options,
        CancellationToken cancellationToken)
    {
        TEvent? message;

        try
        {
            message = JsonSerializer.Deserialize<TEvent>(result.Message.Value);
        }
        catch (JsonException exception)
        {
            logger.LogError(
                exception,
                "Consumer {Consumer} failed to deserialize a message.",
                ConsumerName);

            await DeadLetterAsync(result, exception, logger, options, cancellationToken);
            consumer.Commit(result);
            return;
        }

        if (message is null)
        {
            await DeadLetterAsync(
                result,
                new InvalidOperationException("The message deserialized to null."),
                logger,
                options,
                cancellationToken);

            consumer.Commit(result);
            return;
        }

        using var logScope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["MessageId"] = message.MessageId,
            ["CorrelationId"] = message.CorrelationId,
            ["EventId"] = message.EventId
        });

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<IOperationsDbContext>();

                var alreadyProcessed = await db.ProcessedMessages.AnyAsync(
                    x => x.Consumer == ConsumerName && x.MessageId == message.MessageId,
                    cancellationToken);

                if (alreadyProcessed)
                {
                    logger.LogInformation(
                        "Consumer {Consumer} skipping duplicate message {MessageId}.",
                        ConsumerName,
                        message.MessageId);

                    consumer.Commit(result);
                    return;
                }

                await HandleMessageAsync(message, db, cancellationToken);

                db.ProcessedMessages.Add(
                    new ProcessedMessage(ConsumerName, message.MessageId, DateTime.UtcNow));

                await db.SaveChangesAsync(cancellationToken);

                consumer.Commit(result);

                logger.LogInformation(
                    "Consumer {Consumer} processed message {MessageId}.",
                    ConsumerName,
                    message.MessageId);

                return;
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (attempt >= options.ConsumerMaxAttempts)
                {
                    logger.LogError(
                        exception,
                        "Consumer {Consumer} failed message {MessageId} after {Attempts} attempts.",
                        ConsumerName,
                        message.MessageId,
                        attempt);

                    await DeadLetterAsync(result, exception, logger, options, cancellationToken);
                    consumer.Commit(result);
                    return;
                }

                logger.LogWarning(
                    exception,
                    "Consumer {Consumer} failed message {MessageId} (attempt {Attempt}/{Max}).",
                    ConsumerName,
                    message.MessageId,
                    attempt,
                    options.ConsumerMaxAttempts);

                await Task.Delay(
                    TimeSpan.FromSeconds(options.ConsumerRetryBackoffSeconds),
                    cancellationToken);
            }
        }
    }

    // Publishes the original message to the dead-letter topic. If that publish
    // fails the exception propagates so the caller does not commit the offset.
    private async Task DeadLetterAsync(
        ConsumeResult<string, string> result,
        Exception exception,
        ILogger logger,
        KafkaOptions options,
        CancellationToken cancellationToken)
    {
        var deadLetterTopic = $"{Topic}{options.DeadLetterSuffix}";
        var key = result.Message.Key ?? Guid.NewGuid().ToString();

        await producer.PublishAsync(deadLetterTopic, key, result.Message.Value, cancellationToken);

        logger.LogError(
            exception,
            "Consumer {Consumer} sent message {Key} to dead letter topic {DeadLetterTopic}.",
            ConsumerName,
            key,
            deadLetterTopic);
    }
}
