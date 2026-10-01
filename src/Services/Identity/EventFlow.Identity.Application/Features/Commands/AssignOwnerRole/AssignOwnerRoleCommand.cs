using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.AssignOwnerRole;

public sealed record AssignOwnerRoleCommand(Guid UserId, Guid EventId) : IRequest<Guid>;
