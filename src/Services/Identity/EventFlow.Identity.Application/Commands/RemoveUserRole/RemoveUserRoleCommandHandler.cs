using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Commands.RemoveUserRole
{
    public sealed class RemoveUserRoleCommandHandler(IUserEventRoleRepository userEventRoleRepository) : IRequestHandler<RemoveUserRoleCommand>
    {

        public async Task Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
        {
            // Find the user's role assignment for this event.
            var roles = await userEventRoleRepository.GetByUserAndEventAsync(request.UserId,request.EventId,cancellationToken);

            var userEventRole = roles.FirstOrDefault(x => x.RoleId == request.RoleId);

            if (userEventRole is null)
            {
                throw new NotFoundException("Role assignment was not found.");
            }

            // Remove the role assignment.
            await userEventRoleRepository.DeleteAsync(userEventRole);
            await userEventRoleRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
