using EventFlow.Operations.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Operations.Domain.Entities;

public sealed class Notification : Entity
{
    // EF Core materialization constructor.
    private Notification()
    {
    }

    public Notification(
        Guid eventId,
        Guid? userId,
        string recipientEmail,
        string subject,
        string body,
        DateTime scheduledAtUtc)
    {
        EventId = eventId;
        UserId = userId;
        RecipientEmail = recipientEmail;
        Subject = subject;
        Body = body;
        Status = NotificationStatus.Pending;
        ScheduledAtUtc = scheduledAtUtc;
    }

    public Guid EventId { get; private set; }

    public Guid? UserId { get; private set; }

    public string RecipientEmail { get; private set; } = string.Empty;

    public string Subject { get; private set; } = string.Empty;

    public string Body { get; private set; } = string.Empty;

    public NotificationStatus Status { get; private set; } = NotificationStatus.Pending;

    public int AttemptCount { get; private set; }

    public DateTime ScheduledAtUtc { get; private set; }

    public DateTime? SentAtUtc { get; private set; }

    public DateTime? FailedAtUtc { get; private set; }

    public string? Error { get; private set; }

    // Marks the notification as picked up for delivery.
    public void MarkProcessing()
    {
        Status = NotificationStatus.Processing;
        AttemptCount++;
        SetUpdatedAt();
    }

    // Marks the notification as successfully delivered.
    public void MarkSent()
    {
        Status = NotificationStatus.Sent;
        SentAtUtc = DateTime.UtcNow;
        FailedAtUtc = null;
        Error = null;
        SetUpdatedAt();
    }

    // Marks the notification as permanently failed.
    public void MarkFailed(string error)
    {
        Error = error;
        Status = NotificationStatus.Failed;
        FailedAtUtc = DateTime.UtcNow;
        SetUpdatedAt();
    }

    // Schedules a retry after a transient failure.
    public void ScheduleRetry(string error)
    {
        Error = error;
        Status = NotificationStatus.Pending;
        SetUpdatedAt();
    }

    // Reverts to pending, undoing the attempt counter (e.g. worker shutdown).
    public void RevertToPending()
    {
        Status = NotificationStatus.Pending;
        if (AttemptCount > 0)
        {
            AttemptCount--;
        }

        SetUpdatedAt();
    }
}
