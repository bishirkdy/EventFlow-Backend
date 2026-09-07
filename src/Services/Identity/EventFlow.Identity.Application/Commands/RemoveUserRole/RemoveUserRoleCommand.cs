using MediatR;

namespace EventFlow.Identity.Application.Commands.RemoveUserRole
{
    public record RemoveUserRoleCommand(Guid UserId,Guid EventId,Guid RoleId) : IRequest;
}
