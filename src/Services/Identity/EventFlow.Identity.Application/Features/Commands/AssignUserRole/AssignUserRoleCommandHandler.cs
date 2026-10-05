using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.AssignUserRole;

public sealed class AssignUserRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserEventRoleRepository userEventRoleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<AssignUserRoleCommand, Guid>
{
    public async Task<Guid> Handle(
        AssignUserRoleCommand request,
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

        var user = await userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var role = await roleRepository.GetByIdAsync(
            request.RoleId,
            cancellationToken);

        if (role is null)
        {
            throw new NotFoundException("Role not found.");
        }

        var alreadyExists = await userEventRoleRepository.ExistsAsync(
            request.UserId,
            request.EventId,
            request.RoleId,
            cancellationToken);

        if (alreadyExists)
        {
            throw new ConflictException("This role is already assigned to the user for this event.");
        }

        var userEventRole = new UserEventRole(
            request.UserId,
            request.EventId,
            request.RoleId);

        await userEventRoleRepository.AddAsync(userEventRole, cancellationToken);
        await userEventRoleRepository.SaveChangesAsync(cancellationToken);

        return userEventRole.Id;
    }
}
