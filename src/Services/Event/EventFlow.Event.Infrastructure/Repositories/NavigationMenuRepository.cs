
using Microsoft.EntityFrameworkCore;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class NavigationMenuRepository(EventCoreDbContext context): GenericRepository<NavigationMenu>(context),
          INavigationMenuRepository
    {
        public async Task<IReadOnlyList<NavigationMenu>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            // Get navigation menus belonging to event
            return await Context.NavigationMenus
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
