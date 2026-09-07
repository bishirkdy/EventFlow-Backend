
using MediatR;

namespace EventFlow.Identity.Application.Commands.AssignUserRole
{
    public record AssignUserRoleCommand(Guid UserId,Guid EventId,Guid RoleId) : IRequest<Guid>;
}
