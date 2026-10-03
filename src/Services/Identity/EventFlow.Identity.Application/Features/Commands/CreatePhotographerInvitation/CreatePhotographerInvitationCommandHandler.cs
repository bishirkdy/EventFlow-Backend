using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.CreatePhotographerInvitation
{
    public class CreatePhotographerInvitationCommandHandler : IRequestHandler<CreatePhotographerInvitationCommand, CreatePhotographerInvitationResponse>
    {
        private readonly IPhotographerInvitationRepository _invitationRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public CreatePhotographerInvitationCommandHandler(
            IPhotographerInvitationRepository invitationRepository,
            IRoleRepository roleRepository,
            IUserRepository userRepository)
        {
            _invitationRepository = invitationRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<CreatePhotographerInvitationResponse> Handle(CreatePhotographerInvitationCommand request, CancellationToken cancellationToken)
        {
            // Verify the role exists
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null)
            {
                throw new NotFoundException("Role not found.");
            }

            // Check if there's already a pending invitation for this email and event
            var existingInvitation = await _invitationRepository.GetByEventAndEmailAsync(request.EventId, request.Email, cancellationToken);
            if (existingInvitation is not null && existingInvitation.Status == InvitationStatus.Pending)
            {
                throw new ConflictException("A pending invitation already exists for this email and event.");
            }

            // Create the invitation
            var invitation = new PhotographerInvitation(request.EventId, request.Email, request.RoleId, request.CreatedBy);
            await _invitationRepository.AddAsync(invitation, cancellationToken);

            return new CreatePhotographerInvitationResponse(invitation.Id, invitation.Token, invitation.ExpiresAt);
        }
    }
}