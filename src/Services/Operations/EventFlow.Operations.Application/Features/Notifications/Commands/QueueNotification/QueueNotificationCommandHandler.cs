using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Operations.Application.Features.Notifications.Commands.QueueNotification;

public sealed class QueueNotificationCommandHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    ICurrentUserService currentUser)
    : IRequestHandler<QueueNotificationCommand, NotificationDto>
{
    public async Task<NotificationDto> Handle(
        QueueNotificationCommand command,
        CancellationToken cancellationToken)
    {
        if (!await authorization.HasPermissionAsync(
                currentUser.UserId,
                command.EventId,
                "event.update",
                cancellationToken))
        {
            throw new ForbiddenException(
                "You do not have permission to send event notifications.");
        }

        var entity = new Notification
        {
            EventId = command.EventId,
            UserId = command.UserId,
            RecipientEmail = command.RecipientEmail.Trim(),
            Subject = command.Subject.Trim(),
            Body = command.Body,
            ScheduledAtUtc = command.ScheduledAtUtc ?? DateTime.UtcNow
        };

        db.Notifications.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new NotificationDto(
            entity.Id,
            entity.EventId,
            entity.UserId,
            entity.RecipientEmail,
            entity.Subject,
            entity.Status,
            entity.AttemptCount,
            entity.ScheduledAtUtc,
            entity.SentAtUtc,
            entity.Error);
    }
}
