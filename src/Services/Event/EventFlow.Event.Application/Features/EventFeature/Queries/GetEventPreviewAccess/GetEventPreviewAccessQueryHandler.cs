using EventFlow.Security.Authorization;
using MediatR;
using EventAuthorizationService = EventFlow.Event.Application.Abstractions.Authorization.IAuthorizationService;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetEventPreviewAccess;

// Preview is limited to the roles that manage an event: the owner
// (event.team.manage) and the organizer/event admin (event.update).
public sealed class GetEventPreviewAccessQueryHandler(EventAuthorizationService permissions)
    : IRequestHandler<GetEventPreviewAccessQuery, bool>
{
    public async Task<bool> Handle(
        GetEventPreviewAccessQuery request,
        CancellationToken cancellationToken)
    {
        var canUpdate = await permissions.HasPermissionAsync(
            request.UserId,
            request.EventId,
            PermissionConstants.Event.Update,
            cancellationToken);

        if (canUpdate)
        {
            return true;
        }

        return await permissions.HasPermissionAsync(
            request.UserId,
            request.EventId,
            PermissionConstants.Event.TeamManage,
            cancellationToken);
    }
}
