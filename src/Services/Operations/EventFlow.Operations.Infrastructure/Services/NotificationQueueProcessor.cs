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

            notification.Status = NotificationStatus.Processing;
            notification.AttemptCount++;
            await db.SaveChangesAsync(cancellationToken);

            try
            {
                await emailSender.SendAsync(
                    notification.RecipientEmail,
                    notification.Subject,
                    notification.Body,
                    cancellationToken);

                notification.Status = NotificationStatus.Sent;
                notification.SentAtUtc = DateTime.UtcNow;
                notification.FailedAtUtc = null;
                notification.Error = null;
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                notification.Status = NotificationStatus.Pending;
                notification.AttemptCount--;
                await db.SaveChangesAsync(cancellationToken);
                throw;
            }
            catch (Exception exception)
            {
                notification.Error = Truncate(exception.Message, 1000);

                if (notification.AttemptCount >= MaxAttempts)
                {
                    notification.Status = NotificationStatus.Failed;
                    notification.FailedAtUtc = DateTime.UtcNow;
                }
                else
                {
                    notification.Status = NotificationStatus.Pending;
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
