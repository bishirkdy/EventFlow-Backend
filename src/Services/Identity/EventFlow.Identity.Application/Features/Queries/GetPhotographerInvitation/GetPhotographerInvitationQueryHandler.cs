using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetPhotographerInvitation;

public sealed class GetPhotographerInvitationQueryHandler(
    IPhotographerInvitationRepository invitationRepository)
    : IRequestHandler<GetPhotographerInvitationQuery, GetPhotographerInvitationResponse>
{
    public async Task<GetPhotographerInvitationResponse> Handle(
        GetPhotographerInvitationQuery request,
        CancellationToken cancellationToken)
    {
        var invitation = await invitationRepository.GetByTokenAsync(
            request.Token,
            cancellationToken);

        if (invitation is null)
        {
            throw new NotFoundException("Invitation not found.");
        }

        return new GetPhotographerInvitationResponse(
            invitation.Id,
            invitation.EventId,
            string.Empty,
            invitation.Email,
            invitation.Role?.Name ?? string.Empty,
            invitation.ExpiresAt,
            invitation.Status);
    }
}
