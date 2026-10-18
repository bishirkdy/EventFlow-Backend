using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.UserEventRoles;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetMyEventPermissions;

public sealed class GetMyEventPermissionsQueryHandler(IUserEventRoleRepository userEventRoleRepository)
    : IRequestHandler<GetMyEventPermissionsQuery, EventAccessResponse>
{
    public async Task<EventAccessResponse> Handle(GetMyEventPermissionsQuery request, CancellationToken cancellationToken)
    {
        var userEventRoles = await userEventRoleRepository.GetByUserAndEventAsync(
            request.UserId,
            request.EventId,
            cancellationToken);

        var permissions = await userEventRoleRepository.GetPermissionNamesAsync(
            request.UserId,
            request.EventId,
            cancellationToken);

        var roles = userEventRoles
            .Select(x => x.Role.Name)
            .Distinct()
            .ToList();

        return new EventAccessResponse(roles, permissions);
    }
}
