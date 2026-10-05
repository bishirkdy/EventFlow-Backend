using EventFlow.Operations.Application.Contracts;
using MediatR;

namespace EventFlow.Operations.Application.Features.Notifications.Commands.QueueNotification;

public sealed record QueueNotificationCommand(
    Guid EventId,
    Guid? UserId,
    string RecipientEmail,
    string Subject,
    string Body,
    DateTime? ScheduledAtUtc) : IRequest<NotificationDto>;
