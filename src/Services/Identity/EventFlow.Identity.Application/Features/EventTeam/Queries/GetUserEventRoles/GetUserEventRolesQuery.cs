using EventFlow.Identity.Application.DTOs.UserEventRoles;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetUserEventRoles;

public sealed record GetUserEventRolesQuery(Guid UserId, Guid EventId) : IRequest<List<UserEventRoleResponse>>;
