using EventFlow.Identity.Application.DTOs.UserEventRoles;
using MediatR;
    

namespace EventFlow.Identity.Application.Queries.GetUserEventRoles
{
    //Query to get user role of event
    public sealed record GetUserEventRolesQuery(Guid UserId,Guid EventId) : IRequest<List<UserEventRoleResponse>>;
}
