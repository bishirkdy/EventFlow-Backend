
using Microsoft.EntityFrameworkCore;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class NavigationItemRepository(EventCoreDbContext context): GenericRepository<NavigationItem>(context), INavigationItemRepository
    {
        public async Task<IReadOnlyList<NavigationItem>> GetByMenuIdAsync(Guid navigationMenuId, CancellationToken cancellationToken = default)
        {
            // Get navigation items belonging to menu
            return await context.NavigationItems
                .Where(x => x.NavigationMenuId == navigationMenuId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<NavigationItem?> GetByPageIdAsync(Guid pageId,CancellationToken cancellationToken)
        {
            return await context.NavigationItems
                .Include(x => x.NavigationMenu)
                .FirstOrDefaultAsync(x => x.PageId == pageId, cancellationToken);
        }

    }
}
