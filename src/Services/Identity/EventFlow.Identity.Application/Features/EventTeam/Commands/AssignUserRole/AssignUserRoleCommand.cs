using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Commands.AssignUserRole
{
    //Add role for user by event
    public sealed record AssignUserRoleCommand(Guid UserId,Guid EventId,Guid RoleId) : IRequest<Guid>;
}
