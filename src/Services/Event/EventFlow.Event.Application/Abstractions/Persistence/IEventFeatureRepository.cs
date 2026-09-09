using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IEventFeatureRepository: IRepository<EventFeature>
    {
        Task<EventFeature?> GetByEventAndFeatureAsync(Guid eventId,Guid featureId,CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EventFeature>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
