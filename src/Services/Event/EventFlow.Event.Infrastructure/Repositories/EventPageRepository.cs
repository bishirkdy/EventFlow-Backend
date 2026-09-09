using Microsoft.EntityFrameworkCore;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class EventPageRepository(EventCoreDbContext context): GenericRepository<EventPage>(context), IEventPageRepository
    {
        public async Task<IReadOnlyList<EventPage>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            // Get pages belonging to event
            return await Context.EventPages
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<EventPage?> GetBySlugAsync(Guid eventId,string slug,CancellationToken cancellationToken = default)
        {
            // Get page by event and slug
            return await Context.EventPages.FirstOrDefaultAsync(x => x.EventId == eventId && x.Slug == slug, cancellationToken);
        }
    }
}
