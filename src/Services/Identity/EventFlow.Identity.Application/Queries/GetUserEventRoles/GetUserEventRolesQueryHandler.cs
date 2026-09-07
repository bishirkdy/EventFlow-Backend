using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.UserEventRoles;
using MediatR;

namespace EventFlow.Identity.Application.Queries.GetUserEventRoles
{
    public sealed class GetUserEventRolesQueryHandler(IUserEventRoleRepository userEventRoleRepository) : IRequestHandler<GetUserEventRolesQuery, List<UserEventRoleResponse>>
    {


        public async Task<List<UserEventRoleResponse>> Handle(GetUserEventRolesQuery request, CancellationToken cancellationToken)
        {
            // Get all roles assigned to the user for this event.
            var userEventRoles = await userEventRoleRepository.GetByUserAndEventAsync(request.UserId,request.EventId,cancellationToken);

            // Return only the data needed by the API.
            return userEventRoles
                .Select(x => new UserEventRoleResponse(
                    x.Id,
                    x.RoleId,
                    x.Role.Name,
                    x.EventId))
                .ToList();
        }
    }
}
