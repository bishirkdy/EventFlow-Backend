using EventFlow.Messaging.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventFlow.Messaging.Outbox;

// Background publisher that drains the outbox and pushes each message to Kafka,
// retrying with backoff and marking rows as processed or failed.
public sealed class OutboxPublisher<TContext>(
    IServiceScopeFactory scopeFactory,
    IKafkaProducer producer,
    IOptions<OutboxOptions> options,
    ILogger<OutboxPublisher<TContext>> logger) : BackgroundService
    where TContext : DbContext, IOutboxDbContext
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        logger.LogInformation("Outbox publisher for {Context} started.", typeof(TContext).Name);

        var pollInterval = TimeSpan.FromSeconds(settings.PollIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await PublishDueAsync(settings, stoppingToken);

                if (processed == 0)
                {
                    await Task.Delay(pollInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Outbox publisher loop failed.");

                try
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(settings.RetryBackoffSeconds),
                        stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        logger.LogInformation("Outbox publisher for {Context} stopped.", typeof(TContext).Name);
    }

    private async Task<int> PublishDueAsync(OutboxOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        var now = DateTime.UtcNow;
        var lockThreshold = now.AddSeconds(-settings.LockTimeoutSeconds);

        var due = await context.OutboxMessages
            .Where(x =>
                (x.Status == OutboxStatus.Pending &&
                 (x.NextAttemptAtUtc == null || x.NextAttemptAtUtc <= now)) ||
                (x.Status == OutboxStatus.Processing &&
                 x.LockedAtUtc != null &&
                 x.LockedAtUtc < lockThreshold))
            .OrderBy(x => x.OccurredAtUtc)
            .Take(settings.BatchSize)
            .ToListAsync(cancellationToken);

        if (due.Count == 0)
        {
            return 0;
        }

        foreach (var message in due)
        {
            cancellationToken.ThrowIfCancellationRequested();

            message.MarkProcessing(now);
            await context.SaveChangesAsync(cancellationToken);

            try
            {
                await producer.PublishAsync(
                    message.Topic,
                    message.EventContextId.ToString(),
                    message.Payload,
                    cancellationToken);

                message.MarkProcessed(DateTime.UtcNow);

                logger.LogInformation(
                    "Published outbox message {MessageId} to {Topic}.",
                    message.MessageId,
                    message.Topic);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                var error = Truncate(exception.Message, 1000);

                if (message.Attempts >= settings.MaxAttempts)
                {
                    message.MarkFailed(error);

                    logger.LogError(
                        exception,
                        "Outbox message {MessageId} exhausted its attempts.",
                        message.MessageId);
                }
                else
                {
                    message.ScheduleRetry(
                        error,
                        DateTime.UtcNow.AddSeconds(settings.RetryBackoffSeconds));

                    logger.LogWarning(
                        exception,
                        "Outbox message {MessageId} publish failed, will retry.",
                        message.MessageId);
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        return due.Count;
    }

    private static string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
