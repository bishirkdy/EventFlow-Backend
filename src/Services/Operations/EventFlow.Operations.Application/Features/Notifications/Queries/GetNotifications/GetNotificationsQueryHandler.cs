using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Notifications.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    ICurrentUserService currentUser)
    : IRequestHandler<GetNotificationsQuery, IReadOnlyList<NotificationDto>>
{
    public async Task<IReadOnlyList<NotificationDto>> Handle(
        GetNotificationsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await authorization.HasPermissionAsync(
                currentUser.UserId,
                query.EventId,
                "event.view",
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to view notifications.");
        }

        var list = await db.Notifications
            .Where(x => x.EventId == query.EventId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(100)
            .ToListAsync(cancellationToken);

        return list.Select(x => new NotificationDto(
            x.Id,
            x.EventId,
            x.UserId,
            x.RecipientEmail,
            x.Subject,
            x.Status,
            x.AttemptCount,
            x.ScheduledAtUtc,
            x.SentAtUtc,
            x.Error)).ToList();
    }
}
