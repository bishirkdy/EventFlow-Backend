using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.UserEventRoles;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetUserEventRoles;

public sealed class GetUserEventRolesQueryHandler(IUserEventRoleRepository userEventRoleRepository,IPermissionService permissions,ICurrentUserService currentUser)
    : IRequestHandler<GetUserEventRolesQuery, List<UserEventRoleResponse>>
{
    public async Task<List<UserEventRoleResponse>> Handle(GetUserEventRolesQuery request,CancellationToken cancellationToken)
    {
        // Users may retrieve their own roles without event.view permission.
        // Retrieving another user's roles requires event.view permission.
        if (request.UserId != currentUser.UserId &&
            !await permissions.HasPermissionAsync(currentUser.UserId,request.EventId,PermissionConstants.Event.View,cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to view this user's roles.");
        }

        var userEventRoles =
            await userEventRoleRepository.GetByUserAndEventAsync(request.UserId,request.EventId,cancellationToken);

        return userEventRoles
            .Select(x => new UserEventRoleResponse(
                x.Id,
                x.RoleId,
                x.Role.Name,
                x.EventId))
            .ToList();
    }
}