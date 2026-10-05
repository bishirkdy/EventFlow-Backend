using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RemoveOrganizer;

public sealed class RemoveOrganizerCommandHandler(
    IRoleRepository roleRepository,
    IUserEventRoleRepository userEventRoleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<RemoveOrganizerCommand>
{
    public async Task Handle(
        RemoveOrganizerCommand request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                request.EventId,
                PermissionConstants.Event.TeamManage,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to manage the event team.");
        }

        var organizerRole = await roleRepository.GetByNameAsync(
            RoleConstants.Organizer,
            cancellationToken);

        if (organizerRole is null)
        {
            throw new ConflictException("The Organizer role is not configured.");
        }

        var assignments = await userEventRoleRepository.GetByUserAndEventAsync(
            request.UserId,
            request.EventId,
            cancellationToken);

        var assignment = assignments.FirstOrDefault(x => x.RoleId == organizerRole.Id);

        if (assignment is null)
        {
            throw new NotFoundException("Organizer assignment was not found.");
        }

        await userEventRoleRepository.DeleteAsync(assignment);
        await userEventRoleRepository.SaveChangesAsync(cancellationToken);
    }
}
