
using Microsoft.EntityFrameworkCore;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories
{
    public sealed class NavigationItemRepository(EventCoreDbContext context): GenericRepository<NavigationItem>(context),   INavigationItemRepository
    {
        public async Task<IReadOnlyList<NavigationItem>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken)
        {
            return await context.NavigationItems
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<NavigationItem?> GetByPageIdAsync(Guid pageId,CancellationToken cancellationToken)
        {
            return await context.NavigationItems
                .FirstOrDefaultAsync(x => x.PageId == pageId, cancellationToken);
        }

    }
}
