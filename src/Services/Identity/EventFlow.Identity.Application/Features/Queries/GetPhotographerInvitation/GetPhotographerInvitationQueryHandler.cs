using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetPhotographerInvitation
{
    public class GetPhotographerInvitationQueryHandler : IRequestHandler<GetPhotographerInvitationQuery, GetPhotographerInvitationResponse?>
    {
        private readonly IPhotographerInvitationRepository _invitationRepository;

        public GetPhotographerInvitationQueryHandler(IPhotographerInvitationRepository invitationRepository)
        {
            _invitationRepository = invitationRepository;
        }

        public async Task<GetPhotographerInvitationResponse?> Handle(GetPhotographerInvitationQuery request, CancellationToken cancellationToken)
        {
            var invitation = await _invitationRepository.GetByTokenAsync(request.Token, cancellationToken);
            if (invitation is null)
            {
                return null;
            }

            return new GetPhotographerInvitationResponse(
                invitation.Id,
                invitation.EventId,
                string.Empty, // Event name would require cross-service call
                invitation.Email,
                invitation.Role?.Name ?? string.Empty,
                invitation.ExpiresAt,
                invitation.Status);
        }
    }
}