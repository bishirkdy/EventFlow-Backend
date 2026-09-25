using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RemoveUserRole
{
    public record RemoveUserRoleCommand(Guid UserId,Guid EventId,Guid RoleId) : IRequest;
}
