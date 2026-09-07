

using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Exceptions;
using EventFlow.Identity.Domain.Entities;
using MediatR;

namespace EventFlow.Identity.Application.Commands.AssignUserRole
{
    public class AssignUserRoleCommandHandler
      : IRequestHandler<AssignUserRoleCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserEventRoleRepository _userEventRoleRepository;

        public AssignUserRoleCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserEventRoleRepository userEventRoleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userEventRoleRepository = userEventRoleRepository;
        }

        public async Task<Guid> Handle(
            AssignUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            // Purpose: Make sure the user exists.
            var user = await _userRepository.GetByIdAsync(
                request.UserId,
                cancellationToken);

            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }

            // Purpose: Make sure the role exists.
            var role = await _roleRepository.GetByIdAsync(
                request.RoleId,
                cancellationToken);

            if (role is null)
            {
                throw new NotFoundException("Role not found.");
            }

            // Purpose: Prevent duplicate role assignment.
            var alreadyExists =
                await _userEventRoleRepository.ExistsAsync(
                    request.UserId,
                    request.EventId,
                    request.RoleId,
                    cancellationToken);

            if (alreadyExists)
            {
                throw new ConflictException(
                    "This role is already assigned to the user for this event.");
            }

            // Purpose: Create the user-event-role relationship.
            var userEventRole = new UserEventRole(
                request.UserId,
                request.EventId,
                request.RoleId);

            await _userEventRoleRepository.AddAsync(
                userEventRole,
                cancellationToken);

            // Purpose: Save the role assignment.
            await _userEventRoleRepository.SaveChangesAsync(
                cancellationToken);

            return userEventRole.Id;
        }
    }
}
