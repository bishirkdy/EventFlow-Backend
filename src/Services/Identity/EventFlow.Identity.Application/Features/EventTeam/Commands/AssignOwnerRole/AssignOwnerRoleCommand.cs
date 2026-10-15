using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Commands.AssignOwnerRole;

public sealed record AssignOwnerRoleCommand(Guid UserId, Guid EventId) : IRequest<Guid>;
