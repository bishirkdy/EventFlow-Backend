using AutoMapper;
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
    ICurrentUserService currentUser,
    IMapper mapper)
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

        var entity = new Notification(
            command.EventId,
            command.UserId,
            command.RecipientEmail.Trim(),
            command.Subject.Trim(),
            command.Body,
            command.ScheduledAtUtc ?? DateTime.UtcNow);

        db.Notifications.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<NotificationDto>(entity);
    }
}
