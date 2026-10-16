using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Abstractions.ReadModels;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence.Repositories
{
    // Implements the user persistence operations defined by IUserRepository.
    public sealed class UserRepository(IdentityDbContext context) : IUserRepository
    {
        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Email == email,cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(string userName,CancellationToken cancellationToken)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        }

        public async Task AddAsync(User user,CancellationToken cancellationToken)
        {
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user,CancellationToken cancellationToken)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email,CancellationToken cancellationToken)
        {
            return await context.Users.AnyAsync(x => x.Email == email,cancellationToken);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            return await context.Users.AnyAsync(x => x.UserName == userName,cancellationToken);
        }

        // Get users by pagination for the admin console.
        public async Task<PaginatedResponse<User>> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            CancellationToken cancellationToken)
        {
            var query = context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(x =>
                    EF.Functions.ILike(x.Email, $"%{term}%") ||
                    EF.Functions.ILike(x.UserName, $"%{term}%") ||
                    EF.Functions.ILike(x.FirstName, $"%{term}%") ||
                    EF.Functions.ILike(x.LastName, $"%{term}%"));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResponse<User>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // Get aggregate user counts in a single query.
        public async Task<UserCountStats> GetCountsAsync(CancellationToken cancellationToken)
        {
            var counts = await context.Users
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(group => new UserCountStats(
                    group.Count(),
                    group.Count(x => x.IsActive),
                    group.Count(x => x.IsPlatformAdmin)))
                .FirstOrDefaultAsync(cancellationToken);

            return counts ?? new UserCountStats(0, 0, 0);
        }
    }
}
