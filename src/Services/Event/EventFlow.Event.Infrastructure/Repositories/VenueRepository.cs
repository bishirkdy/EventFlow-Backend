

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class VenueRepository(EventCoreDbContext context): GenericRepository<Venue>(context), IVenueRepository
    {
        public async Task<IReadOnlyList<Venue>> GetByEventIdAsync(Guid eventId,CancellationToken cancellationToken = default)
        {
            // Get venues belonging to event
            return await Context.Venues
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
