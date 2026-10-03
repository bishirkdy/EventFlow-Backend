using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RevokePhotographerInvitation
{
    public class RevokePhotographerInvitationCommandHandler : IRequestHandler<RevokePhotographerInvitationCommand>
    {
        private readonly IPhotographerInvitationRepository _invitationRepository;

        public RevokePhotographerInvitationCommandHandler(IPhotographerInvitationRepository invitationRepository)
        {
            _invitationRepository = invitationRepository;
        }

        public async Task Handle(RevokePhotographerInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);
            if (invitation is null)
            {
                throw new NotFoundException("Invitation not found.");
            }

            invitation.Revoke();
            await _invitationRepository.UpdateAsync(invitation, cancellationToken);
        }
    }
}