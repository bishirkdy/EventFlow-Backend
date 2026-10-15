using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Commands.RemoveUserRole
{
    public record RemoveUserRoleCommand(Guid UserId,Guid EventId,Guid RoleId) : IRequest;
}
