

namespace EventFlow.Identity.Application.DTOs.UserEventRoles
{
    public sealed record UserEventRoleResponse(Guid Id,Guid RoleId,string RoleName,Guid EventId);
}
