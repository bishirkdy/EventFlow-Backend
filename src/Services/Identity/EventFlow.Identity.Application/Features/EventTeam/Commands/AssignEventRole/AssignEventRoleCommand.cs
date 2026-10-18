using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Commands.AssignEventRole;

public sealed record AssignEventRoleCommand(Guid UserId, Guid EventId, string RoleName) : IRequest<Guid>;
