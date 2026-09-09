using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class EventFeatureRepository(EventCoreDbContext context): GenericRepository<EventFeature>(context), IEventFeatureRepository
    {
        public async Task<EventFeature?> GetByEventAndFeatureAsync(Guid eventId,Guid featureId,CancellationToken cancellationToken = default)
        {
            return await Context.EventFeatures
                .FirstOrDefaultAsync(
                    x => x.EventId == eventId &&
                         x.FeatureId == featureId,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<EventFeature>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await Context.EventFeatures
                .Where(x => x.EventId == eventId)
                .ToListAsync(cancellationToken);
        }
    }
}
