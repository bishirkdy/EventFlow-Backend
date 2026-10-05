using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RemoveUserRole;

public sealed class RemoveUserRoleCommandHandler(
    IUserEventRoleRepository userEventRoleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<RemoveUserRoleCommand>
{
    public async Task Handle(
        RemoveUserRoleCommand request,
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

        var roles = await userEventRoleRepository.GetByUserAndEventAsync(
            request.UserId,
            request.EventId,
            cancellationToken);

        var userEventRole = roles.FirstOrDefault(x => x.RoleId == request.RoleId);

        if (userEventRole is null)
        {
            throw new NotFoundException("Role assignment was not found.");
        }

        await userEventRoleRepository.DeleteAsync(userEventRole);
        await userEventRoleRepository.SaveChangesAsync(cancellationToken);
    }
}
