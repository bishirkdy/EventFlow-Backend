namespace EventFlow.Operations.Application.Contracts;

public sealed record QueueNotificationRequest(
    Guid? UserId,
    string RecipientEmail,
    string Subject,
    string Body,
    DateTime? ScheduledAtUtc);
