using EventFlow.Event.Application.Features.Events.Queries.GetEventIsPublished;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using MediatR;
using EventAuthorizationService =
    EventFlow.Event.Application.Abstractions.Authorization.IAuthorizationService;

namespace EventFlow.Event.Api.Services;

// Preview is limited to the roles that manage an event: the owner
// (event.team.manage) and the organizer/event admin (event.update).
public sealed class EventPreviewAccess(
    ICurrentUserService currentUserService,
    EventAuthorizationService authorizationService,
    ISender sender) : IEventPreviewAccess
{
    private static readonly string[] PreviewPermissions =
    [
        PermissionConstants.Event.Update,
        PermissionConstants.Event.TeamManage
    ];

    public async Task<bool> CanPreviewAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return false;
        }

        if (currentUserService.IsPlatformAdmin)
        {
            return true;
        }

        foreach (var permission in PreviewPermissions)
        {
            if (await authorizationService.HasPermissionAsync(
                    currentUserService.UserId,
                    eventId,
                    permission,
                    cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> CanReadAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var isPublished = await sender.Send(
            new GetEventIsPublishedQuery(eventId),
            cancellationToken);

        if (isPublished)
        {
            return true;
        }

        return await CanPreviewAsync(eventId, cancellationToken);
    }
}
