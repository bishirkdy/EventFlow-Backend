using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(IdentityDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await context.Roles.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<Role> GetOrCreateOwnerRoleAsync(
        CancellationToken cancellationToken = default)
    {
        const string ownerRoleName = "Owner";
        const string ownerPermissionName = "event.team.manage";

        var ownerRole = await context.Roles
            .FirstOrDefaultAsync(x => x.Name == ownerRoleName, cancellationToken);

        if (ownerRole is null)
        {
            ownerRole = new Role(
                ownerRoleName,
                "Event owner with read access to the event and permission to manage the event team.");

            await context.Roles.AddAsync(ownerRole, cancellationToken);
        }

        var eventViewPermission = await context.Permissions
            .FirstOrDefaultAsync(x => x.Name == "event.view", cancellationToken);

        if (eventViewPermission is null)
        {
            eventViewPermission = new Permission(
                "event.view",
                "View event information.");

            await context.Permissions.AddAsync(eventViewPermission, cancellationToken);
        }

        var teamManagePermission = await context.Permissions
            .FirstOrDefaultAsync(x => x.Name == ownerPermissionName, cancellationToken);

        if (teamManagePermission is null)
        {
            teamManagePermission = new Permission(
                ownerPermissionName,
                "Manage the event owner and organizer team.");

            await context.Permissions.AddAsync(teamManagePermission, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        var hasEventView = await context.RolePermissions.AnyAsync(
            x => x.RoleId == ownerRole.Id && x.PermissionId == eventViewPermission.Id,
            cancellationToken);

        if (!hasEventView)
        {
            await context.RolePermissions.AddAsync(
                new RolePermission(ownerRole.Id, eventViewPermission.Id),
                cancellationToken);
        }

        var hasTeamManage = await context.RolePermissions.AnyAsync(
            x => x.RoleId == ownerRole.Id && x.PermissionId == teamManagePermission.Id,
            cancellationToken);

        if (!hasTeamManage)
        {
            await context.RolePermissions.AddAsync(
                new RolePermission(ownerRole.Id, teamManagePermission.Id),
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return ownerRole;
    }
}
