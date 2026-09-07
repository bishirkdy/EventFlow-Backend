using EventFlow.Identity.Application.DTOs.UserEventRoles;
using MediatR;
    

namespace EventFlow.Identity.Application.Queries.GetUserEventRoles
{
    public record GetUserEventRolesQuery(
        Guid UserId,
        Guid EventId
    ) : IRequest<List<UserEventRoleResponse>>;
}
