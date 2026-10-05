using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetEventPhotographerInvitations;

public sealed class GetEventPhotographerInvitationsQueryHandler(
    IPhotographerInvitationRepository invitationRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<GetEventPhotographerInvitationsQuery, IReadOnlyList<GetEventPhotographerInvitationsResponse>>
{
    public async Task<IReadOnlyList<GetEventPhotographerInvitationsResponse>> Handle(
        GetEventPhotographerInvitationsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                request.EventId,
                PermissionConstants.Event.TeamManage,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to manage the event team.");
        }

        var invitations = await invitationRepository.GetByEventIdAsync(
            request.EventId,
            cancellationToken);

        return invitations
            .Select(i => new GetEventPhotographerInvitationsResponse(
                i.Id,
                i.Email,
                i.Role?.Name ?? string.Empty,
                i.Status,
                i.CreatedAt,
                i.ExpiresAt,
                i.AcceptedAt,
                i.AcceptedByUserId))
            .ToList();
    }
}
