
using Microsoft.EntityFrameworkCore;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class EventTypeFeatureRepository(EventCoreDbContext context): GenericRepository<EventTypeFeature>(context),IEventTypeFeatureRepository
    {
        public async Task<IReadOnlyList<EventTypeFeature>>GetByEventTypeIdAsync(Guid eventTypeId,CancellationToken cancellationToken = default)
        {
            return await context.EventTypeFeatures
                .Where(x => x.EventTypeId == eventTypeId)
                .ToListAsync(cancellationToken);
        }
    }
}
