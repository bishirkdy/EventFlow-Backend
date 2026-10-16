using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitations;

public sealed class GetEventInvitationsQueryHandler(
    IEventInvitationRepository invitationRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<GetEventInvitationsQuery, IReadOnlyList<GetEventInvitationsResponse>>
{
    public async Task<IReadOnlyList<GetEventInvitationsResponse>> Handle(
        GetEventInvitationsQuery request,
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
            request.Type,
            cancellationToken);

        return invitations
            .Select(i => new GetEventInvitationsResponse(
                i.Id,
                i.Email,
                i.Role?.Name ?? string.Empty,
                i.Type,
                i.ScopeType,
                i.ScopeId,
                i.Status,
                i.CreatedAt,
                i.ExpiresAt,
                i.AcceptedAt,
                i.AcceptedByUserId))
            .ToList();
    }
}
