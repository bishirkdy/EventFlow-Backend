using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories;

public sealed class NavigationItemRepository : GenericRepository<NavigationItem>, INavigationItemRepository
{
    public NavigationItemRepository(EventCoreDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<NavigationItem>> GetByEventIdAsync(
        Guid eventId, CancellationToken cancellationToken)
    {
        return await Context.NavigationItems
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<NavigationItem?> GetByPageIdAsync(
        Guid pageId, CancellationToken cancellationToken)
    {
        return await Context.NavigationItems
            .FirstOrDefaultAsync(x => x.PageId == pageId, cancellationToken);
    }
}
