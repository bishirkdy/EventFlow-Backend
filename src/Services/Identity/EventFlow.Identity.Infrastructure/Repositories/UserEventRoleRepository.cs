using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Repositories
{
    //It is responsible for managing the relationship between a User, an Event, and a Role.
    public sealed class UserEventRoleRepository(IdentityDbContext context) : IUserEventRoleRepository
    {
        public async Task<bool> ExistsAsync(Guid userId,Guid eventId,Guid roleId, CancellationToken cancellationToken = default)
        {
            return await context.UserEventRoles.AnyAsync(x =>x.UserId == userId &&x.EventId == eventId &&x.RoleId == roleId,cancellationToken);
        }

        public async Task AddAsync(UserEventRole userEventRole,CancellationToken cancellationToken = default)
        {
            await context.UserEventRoles.AddAsync(userEventRole,cancellationToken);
        }

        public async Task<List<UserEventRole>> GetByUserAndEventAsync(Guid userId,Guid eventId,CancellationToken cancellationToken = default)
        {
            return await context.UserEventRoles
                .Include(x => x.Role)
                .Where(x =>
                    x.UserId == userId &&
                    x.EventId == eventId)
                .ToListAsync(cancellationToken);
        }

        public Task DeleteAsync(UserEventRole userEventRole)
        {
            context.UserEventRoles.Remove(userEventRole);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
