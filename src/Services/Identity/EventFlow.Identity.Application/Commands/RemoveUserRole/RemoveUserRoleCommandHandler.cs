using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Commands.RemoveUserRole
{
    public class RemoveUserRoleCommandHandler : IRequestHandler<RemoveUserRoleCommand>
    {
        private readonly IUserEventRoleRepository _repository;

        public RemoveUserRoleCommandHandler(
            IUserEventRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(
            RemoveUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            // Purpose: Find the user's role assignment for this event.
            var roles = await _repository.GetByUserAndEventAsync(
                request.UserId,
                request.EventId,
                cancellationToken);

            var userEventRole = roles.FirstOrDefault(
                x => x.RoleId == request.RoleId);

            if (userEventRole is null)
            {
                throw new NotFoundException(
                    "Role assignment was not found.");
            }

            // Purpose: Remove the role assignment.
            await _repository.DeleteAsync(userEventRole);

            // Purpose: Save the change.
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
