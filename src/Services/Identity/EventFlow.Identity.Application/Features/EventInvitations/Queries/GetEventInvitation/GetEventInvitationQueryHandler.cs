using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitation;

public sealed class GetEventInvitationQueryHandler(
    IEventInvitationRepository invitationRepository)
    : IRequestHandler<GetEventInvitationQuery, GetEventInvitationResponse>
{
    public async Task<GetEventInvitationResponse> Handle(
        GetEventInvitationQuery request,
        CancellationToken cancellationToken)
    {
        var invitation = await invitationRepository.GetByTokenAsync(
            request.Token,
            cancellationToken);

        if (invitation is null)
        {
            throw new NotFoundException("Invitation not found.");
        }

        return new GetEventInvitationResponse(
            invitation.Id,
            invitation.EventId,
            string.Empty,
            invitation.Email,
            invitation.Role?.Name ?? string.Empty,
            invitation.Type,
            invitation.ScopeType,
            invitation.ScopeId,
            invitation.ExpiresAt,
            invitation.Status);
    }
}
