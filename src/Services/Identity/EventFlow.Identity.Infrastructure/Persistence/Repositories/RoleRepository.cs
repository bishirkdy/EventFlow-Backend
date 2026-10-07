using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(IdentityDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Roles.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<Role> GetOrCreateOwnerRoleAsync(CancellationToken cancellationToken = default)
    {
        const string ownerRoleName = "Owner";
        const string ownerPermissionName = "event.team.manage";

        // Find the Owner role if it already exists.
        var ownerRole = await context.Roles
            .FirstOrDefaultAsync(x => x.Name == ownerRoleName, cancellationToken);

        // Create the Owner role if it does not exist.
        if (ownerRole is null)
        {
            ownerRole = new Role(
                ownerRoleName,
                "Event owner with read access to the event and permission to manage the event team.");

            await context.Roles.AddAsync(ownerRole,cancellationToken);
        }

        // Find the event.view permission.
        var eventViewPermission = await context.Permissions
            .FirstOrDefaultAsync(x => x.Name == "event.view", cancellationToken);

        // Create the event.view permission if it does not exist.
        if (eventViewPermission is null)
        {
            eventViewPermission = new Permission("event.view", "View event information.");
            await context.Permissions.AddAsync(eventViewPermission,cancellationToken);
        }

        // Find the event.team.manage permission.
        var teamManagePermission = await context.Permissions
            .FirstOrDefaultAsync(x => x.Name == ownerPermissionName,cancellationToken);

        // Create the event.team.manage permission if it does not exist.
        if (teamManagePermission is null)
        {
            teamManagePermission = new Permission(ownerPermissionName,
                "Manage the event owner and organizer team.");

            await context.Permissions.AddAsync(teamManagePermission, cancellationToken);
        }

        // Save the role and permissions so their IDs are generated.
        await context.SaveChangesAsync(cancellationToken);

        // Check whether Owner already has the event.view permission.
        var hasEventView = await context.RolePermissions
            .AnyAsync(x => x.RoleId == ownerRole.Id &&
                     x.PermissionId == eventViewPermission.Id,cancellationToken);

        // Connect Owner role with event.view permission if not already connected.
        if (!hasEventView)
        {
            await context.RolePermissions.AddAsync(
                new RolePermission(ownerRole.Id, eventViewPermission.Id), cancellationToken);
        }

        // Check whether Owner already has the event.team.manage permission.
        var hasTeamManage = await context.RolePermissions
            .AnyAsync(
                x => x.RoleId == ownerRole.Id &&
                     x.PermissionId == teamManagePermission.Id,
                cancellationToken);

        // Connect Owner role with event.team.manage permission if not already connected.
        if (!hasTeamManage)
        {
            await context.RolePermissions.AddAsync(
                new RolePermission(
                    ownerRole.Id,
                    teamManagePermission.Id),
                cancellationToken);
        }

        // Save the role-permission relationships.
        await context.SaveChangesAsync(cancellationToken);

        // Return the Owner role.
        return ownerRole;
    }
}
