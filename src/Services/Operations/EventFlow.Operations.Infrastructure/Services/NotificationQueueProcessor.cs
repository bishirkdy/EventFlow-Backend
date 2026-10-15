using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventFlow.Operations.Infrastructure.Services;

public sealed class NotificationQueueProcessor(IOperationsDbContext db,IEmailSender emailSender,ILogger<NotificationQueueProcessor> logger)
    : INotificationQueueProcessor
{
    private const int BatchSize = 20;
    private const int MaxAttempts = 3;

    public async Task<int> ProcessDueAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var due = await db.Notifications
            .Where(
                x => x.Status == NotificationStatus.Pending && x.ScheduledAtUtc <= now)
            .OrderBy(x => x.ScheduledAtUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (due.Count == 0)
        {
            return 0;
        }

        foreach (var notification in due)
        {
            cancellationToken.ThrowIfCancellationRequested();

            notification.MarkProcessing();
            await db.SaveChangesAsync(cancellationToken);

            try
            {
                await emailSender.SendAsync(
                    notification.RecipientEmail,
                    notification.Subject,
                    notification.Body,
                    cancellationToken);

                notification.MarkSent();
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                notification.RevertToPending();
                await db.SaveChangesAsync(cancellationToken);
                throw;
            }
            catch (Exception exception)
            {
                var error = Truncate(exception.Message, 1000);

                if (notification.AttemptCount >= MaxAttempts)
                {
                    notification.MarkFailed(error);
                }
                else
                {
                    notification.ScheduleRetry(error);
                }
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        return due.Count;
    }

    private static string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength? value : value[..maxLength];
    }
}
