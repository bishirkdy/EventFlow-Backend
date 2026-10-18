using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Commands.AssignUserRole;

public sealed record AssignUserRoleCommand(Guid UserId, Guid EventId, Guid RoleId) : IRequest<Guid>;
