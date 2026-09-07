using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace EventFlow.Identity.Infrastructure.Authorization
{
    public class PermissionService(IdentityDbContext context) : IPermissionService
    {

        //  Find the user's role for this event
        // and check whether that role has the requested permission.
        public async Task<bool> HasPermissionAsync(Guid userId,Guid eventId,string permission,CancellationToken cancellationToken = default)
        {
            return await context.UserEventRoles
                .Where(x =>
                    x.UserId == userId &&
                    x.EventId == eventId)
                .SelectMany(x => x.Role.RolePermissions)
                .AnyAsync(
                    x => x.Permission.Name == permission,
                    cancellationToken);
        }
    }
}
