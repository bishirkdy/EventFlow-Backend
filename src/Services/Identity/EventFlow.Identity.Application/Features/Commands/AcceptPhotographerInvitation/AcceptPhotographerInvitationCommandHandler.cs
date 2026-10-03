using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.AcceptPhotographerInvitation
{
    public class AcceptPhotographerInvitationCommandHandler : IRequestHandler<AcceptPhotographerInvitationCommand, AcceptPhotographerInvitationResponse>
    {
        private readonly IPhotographerInvitationRepository _invitationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserEventRoleRepository _userEventRoleRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AcceptPhotographerInvitationCommandHandler(
            IPhotographerInvitationRepository invitationRepository,
            IUserRepository userRepository,
            IUserEventRoleRepository userEventRoleRepository,
            IPasswordHasher passwordHasher)
        {
            _invitationRepository = invitationRepository;
            _userRepository = userRepository;
            _userEventRoleRepository = userEventRoleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AcceptPhotographerInvitationResponse> Handle(AcceptPhotographerInvitationCommand request, CancellationToken cancellationToken)
        {
            // Get and validate invitation
            var invitation = await _invitationRepository.GetByTokenAsync(request.Token, cancellationToken);
            if (invitation is null)
            {
                throw new NotFoundException("Invalid invitation token.");
            }

            if (invitation.Status != InvitationStatus.Pending)
            {
                throw new ConflictException("Invitation has already been processed or revoked.");
            }

            if (invitation.IsExpired)
            {
                throw new ConflictException("Invitation has expired.");
            }

            // Check if user already exists with this email
            var user = await _userRepository.GetByEmailAsync(invitation.Email, cancellationToken);
            Guid userId;

            if (user is null)
            {
// Create new user
            var passwordHash = _passwordHasher.Hash(request.Password);
                user = new User(request.UserName, invitation.Email, request.FirstName, request.LastName, passwordHash);
                await _userRepository.AddAsync(user, cancellationToken);
                userId = user.Id;
            }
            else
            {
                userId = user.Id;
                // Update user profile if provided
                user.UpdateName(request.FirstName, request.LastName);
                await _userRepository.UpdateAsync(user, cancellationToken);
            }

            // Assign Photographer role for the event
            var alreadyHasRole = await _userEventRoleRepository.ExistsAsync(userId, invitation.EventId, invitation.RoleId, cancellationToken);
            if (!alreadyHasRole)
            {
                var userEventRole = new UserEventRole(userId, invitation.EventId, invitation.RoleId);
                await _userEventRoleRepository.AddAsync(userEventRole, cancellationToken);
            }

            // Mark invitation as accepted
            invitation.Accept(userId);
            await _invitationRepository.UpdateAsync(invitation, cancellationToken);

            return new AcceptPhotographerInvitationResponse(userId, invitation.Id);
        }
    }
}