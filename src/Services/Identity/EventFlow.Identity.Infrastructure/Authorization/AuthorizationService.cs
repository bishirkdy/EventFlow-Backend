using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Authorization;

public sealed class PermissionService(IdentityDbContext context) : IPermissionService
{
    public async Task<bool> HasPermissionAsync(
        Guid userId,
        Guid eventId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        var normalizedPermission = permission.Trim();

        var directPermission = await context.UserEventRoles
            .Where(x => x.UserId == userId && x.EventId == eventId)
            .SelectMany(x => x.Role.RolePermissions)
            .AnyAsync(
                x => x.Permission.Name == normalizedPermission,
                cancellationToken);

        if (directPermission)
        {
            return true;
        }

        // Owner inherits Organizer permissions for the same event only.
        return await context.UserEventRoles
            .Where(x =>
                x.UserId == userId &&
                x.EventId == eventId &&
                x.Role.Name == "Owner")
            .SelectMany(_ => context.Roles
                .Where(role => role.Name == "Organizer")
                .SelectMany(role => role.RolePermissions))
            .AnyAsync(
                x => x.Permission.Name == normalizedPermission,
                cancellationToken);
    }
}
