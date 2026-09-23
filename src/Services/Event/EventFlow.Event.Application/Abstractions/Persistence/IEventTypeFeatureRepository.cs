

using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IEventTypeFeatureRepository : IRepository<EventTypeFeature>
    {
        Task<IReadOnlyList<EventTypeFeature>> GetByEventTypeIdAsync(Guid eventTypeId,CancellationToken cancellationToken = default);
    }
}
