
using Microsoft.EntityFrameworkCore;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class NavigationItemRepository(EventCoreDbContext context): GenericRepository<NavigationItem>(context), INavigationItemRepository
    {
        public async Task<IReadOnlyList<NavigationItem>> GetByMenuIdAsync(
            Guid navigationMenuId,
            CancellationToken cancellationToken = default)
        {
            // Get navigation items belonging to menu
            return await Context.NavigationItems
                .Where(x => x.NavigationMenuId == navigationMenuId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);
        }
    }
}
