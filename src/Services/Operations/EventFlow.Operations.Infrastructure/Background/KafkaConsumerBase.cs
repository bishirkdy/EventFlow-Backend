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
using System.Text.Json;

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

    // How long a single poll waits before the loop checks its liveness again.
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(500);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var logger = loggerFactory.CreateLogger(GetType());
        var options = kafkaOptions.Value;
        var retryDelay = TimeSpan.FromSeconds(options.ConsumerRetryBackoffSeconds);
        var rebuildDelay = TimeSpan.FromSeconds(options.ConsumerRebuildBackoffSeconds);
        var unassignedTimeout = TimeSpan.FromSeconds(options.ConsumerUnassignedRebuildSeconds);

        // A consumer that loses its partitions (for example when the broker
        // restarts) can stay revoked forever without rejoining the group. Each
        // outer iteration owns a consumer; when it is torn down - because it
        // stayed unassigned for too long or failed unexpectedly - a fresh one is
        // built so group membership is re-established and lagged messages are
        // consumed again.
        while (!stoppingToken.IsCancellationRequested)
        {
            IConsumer<string, string>? consumer = null;

            var isAssigned = false;
            var unassignedSince = DateTime.UtcNow;

            try
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = options.BootstrapServers,
                    GroupId = GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                };

                consumer = new ConsumerBuilder<string, string>(config)
                    .SetErrorHandler((_, error) =>
                        logger.LogError("Kafka consumer {Consumer} error: {Reason}", ConsumerName, error.Reason))
                    .SetPartitionsAssignedHandler((_, partitions) =>
                    {
                        isAssigned = true;

                        logger.LogInformation(
                            "Kafka consumer {Consumer} assigned: {Partitions}",
                            ConsumerName,
                            string.Join(", ", partitions));
                    })
                    .SetPartitionsRevokedHandler((_, partitions) =>
                    {
                        isAssigned = false;
                        unassignedSince = DateTime.UtcNow;

                        logger.LogWarning(
                            "Kafka consumer {Consumer} revoked: {Partitions}",
                            ConsumerName,
                            string.Join(", ", partitions));
                    })
                    .Build();

                consumer.Subscribe(Topic);

                logger.LogInformation("Kafka consumer {Consumer} started on {Topic}.", ConsumerName, Topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<string, string>? result;

                    try
                    {
                        result = consumer.Consume(PollInterval);
                    }
                    catch (ConsumeException exception)
                    {
                        // The broker or a subscribed topic may not be ready yet
                        // (for example a topic that has not been created). Keep
                        // the consumer alive and retry instead of stopping it.
                        logger.LogWarning(
                            "Consumer {Consumer} could not consume from {Topic}: {Reason}",
                            ConsumerName,
                            Topic,
                            exception.Error.Reason);

                        await DelayBeforeRetryAsync(retryDelay, stoppingToken);
                        continue;
                    }
                    catch (KafkaException exception)
                    {
                        if (exception.Error.IsFatal)
                        {
                            throw;
                        }

                        logger.LogWarning(
                            exception,
                            "Consumer {Consumer} encountered a transient Kafka error while consuming.",
                            ConsumerName);

                        await DelayBeforeRetryAsync(retryDelay, stoppingToken);
                        continue;
                    }

                    if (result?.Message?.Value is null)
                    {
                        // No message within the poll window. If the consumer has
                        // no partition assignment after a grace period it is
                        // stuck and needs to be rebuilt.
                        if (!isAssigned && DateTime.UtcNow - unassignedSince >= unassignedTimeout)
                        {
                            logger.LogWarning(
                                "Consumer {Consumer} has been without an assignment for {Seconds}s; rebuilding.",
                                ConsumerName,
                                unassignedTimeout.TotalSeconds);

                            break;
                        }

                        continue;
                    }

                    await ProcessMessageAsync(
                        result.Message.Key,
                        result.Message.Value,
                        logger,
                        options,
                        stoppingToken);

                    // The message is only committed once processing finished or
                    // it was parked in the dead-letter topic. A commit failure is
                    // not fatal: the message may simply be reprocessed and is
                    // guarded by the processed-message check.
                    try
                    {
                        consumer.Commit(result);
                    }
                    catch (KafkaException exception)
                    {
                        logger.LogWarning(
                            exception,
                            "Consumer {Consumer} failed to commit an offset; the message may be reprocessed.",
                            ConsumerName);
                    }
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Kafka consumer {Consumer} stopped.", ConsumerName);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Kafka consumer {Consumer} stopped because of an unexpected error; rebuilding.",
                    ConsumerName);
            }
            finally
            {
                if (consumer is not null)
                {
                    try
                    {
                        consumer.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.LogWarning(exception, "Kafka consumer {Consumer} failed to close cleanly.", ConsumerName);
                    }

                    consumer.Dispose();

                    logger.LogInformation("Kafka consumer {Consumer} closed.", ConsumerName);
                }
            }

            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            await DelayBeforeRetryAsync(rebuildDelay, stoppingToken);
        }
    }

    private static async Task DelayBeforeRetryAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delay, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // The host is shutting down; the caller loop will observe the token.
        }
    }

    // Deserializes, de-duplicates, retries and dead-letters a single message.
    // Exposed so the processing pipeline can be tested without a broker.
    public async Task<MessageOutcome> ProcessMessageAsync(
        string? key,
        string value,
        ILogger logger,
        KafkaOptions options,
        CancellationToken cancellationToken)
    {
        TEvent? message;

        try
        {
            message = JsonSerializer.Deserialize<TEvent>(value);
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "Consumer {Consumer} failed to deserialize a message.", ConsumerName);

            await DeadLetterAsync(key, value, exception, logger, options, cancellationToken);
            return MessageOutcome.DeadLettered;
        }

        if (message is null)
        {
            await DeadLetterAsync(
                key,
                value,
                new InvalidOperationException("The message deserialized to null."),
                logger,
                options,
                cancellationToken);

            return MessageOutcome.DeadLettered;
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

                    return MessageOutcome.Duplicate;
                }

                await HandleMessageAsync(message, db, cancellationToken);

                db.ProcessedMessages.Add(
                    new ProcessedMessage(ConsumerName, message.MessageId, DateTime.UtcNow));

                await db.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Consumer {Consumer} processed message {MessageId}.",
                    ConsumerName,
                    message.MessageId);

                return MessageOutcome.Processed;
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

                    await DeadLetterAsync(key, value, exception, logger, options, cancellationToken);
                    return MessageOutcome.DeadLettered;
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
        string? key,
        string value,
        Exception exception,
        ILogger logger,
        KafkaOptions options,
        CancellationToken cancellationToken)
    {
        var deadLetterTopic = $"{Topic}{options.DeadLetterSuffix}";
        var messageKey = key ?? Guid.NewGuid().ToString();

        await producer.PublishAsync(deadLetterTopic, messageKey, value, cancellationToken);

        logger.LogError(
            exception,
            "Consumer {Consumer} sent message {Key} to dead letter topic {DeadLetterTopic}.",
            ConsumerName,
            messageKey,
            deadLetterTopic);
    }
}
