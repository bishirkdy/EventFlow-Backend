using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Application.Contracts;

public sealed record NotificationDto(
    Guid Id,
    Guid EventId,
    Guid? UserId,
    string RecipientEmail,
    string Subject,
    NotificationStatus Status,
    int AttemptCount,
    DateTime ScheduledAtUtc,
    DateTime? SentAtUtc,
    string? Error);
