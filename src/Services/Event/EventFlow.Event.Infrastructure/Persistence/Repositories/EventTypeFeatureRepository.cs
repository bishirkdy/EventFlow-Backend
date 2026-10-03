using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories;

public sealed class EventTypeFeatureRepository : GenericRepository<EventTypeFeature>, IEventTypeFeatureRepository
{
    public EventTypeFeatureRepository(EventCoreDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<EventTypeFeature>> GetByEventTypeIdAsync(
        Guid eventTypeId, CancellationToken cancellationToken = default)
    {
        return await Context.EventTypeFeatures
            .Where(x => x.EventTypeId == eventTypeId)
            .ToListAsync(cancellationToken);
    }
}
