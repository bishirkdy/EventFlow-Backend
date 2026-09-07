using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Repositories
{
    public class UserEventRoleRepository : IUserEventRoleRepository
    {
        private readonly IdentityDbContext _context;

        public UserEventRoleRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(
            Guid userId,
            Guid eventId,
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserEventRoles
                .AnyAsync(
                    x =>
                        x.UserId == userId &&
                        x.EventId == eventId &&
                        x.RoleId == roleId,
                    cancellationToken);
        }

        public async Task AddAsync(
            UserEventRole userEventRole,
            CancellationToken cancellationToken = default)
        {
            await _context.UserEventRoles.AddAsync(
                userEventRole,
                cancellationToken);
        }

        public async Task<List<UserEventRole>> GetByUserAndEventAsync(
            Guid userId,
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserEventRoles
                .Include(x => x.Role)
                .Where(x =>
                    x.UserId == userId &&
                    x.EventId == eventId)
                .ToListAsync(cancellationToken);
        }

        public Task DeleteAsync(UserEventRole userEventRole)
        {
            _context.UserEventRoles.Remove(userEventRole);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
