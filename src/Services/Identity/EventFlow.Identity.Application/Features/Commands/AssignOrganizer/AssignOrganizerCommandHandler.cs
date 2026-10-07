using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.AssignOrganizer;

public sealed class AssignOrganizerCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserEventRoleRepository userEventRoleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<AssignOrganizerCommand, Guid>
{
    public async Task<Guid> Handle(AssignOrganizerCommand request, CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(currentUser.UserId,request.EventId,PermissionConstants.Event.TeamManage, cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to manage the event team.");
        }

        var email = request.Email.Trim();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("No user was found with that email address.");
        }

        var organizerRole = await roleRepository.GetByNameAsync(RoleConstants.Organizer, cancellationToken);

        if (organizerRole is null)
        {
            throw new ConflictException("The Organizer role is not configured.");
        }

        if (user.Id == currentUser.UserId)
        {
            throw new ConflictException("The event owner cannot be assigned as a separate organizer");
        }

        var alreadyAssigned = await userEventRoleRepository.ExistsAsync(user.Id,request.EventId,organizerRole.Id,cancellationToken);

        if (alreadyAssigned)
        {
            throw new ConflictException("This user is already an organizer for this event.");
        }

        var assignment = new UserEventRole(user.Id, request.EventId, organizerRole.Id);

        await userEventRoleRepository.AddAsync(assignment, cancellationToken);
        await userEventRoleRepository.SaveChangesAsync(cancellationToken);

        return assignment.Id;
    }
}
