using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Commands.RemoveEventRole;

public sealed record RemoveEventRoleCommand(
    Guid UserId,
    Guid EventId,
    string RoleName) : IRequest;
