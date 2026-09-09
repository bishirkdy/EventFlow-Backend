

using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface ISectionRepository : IRepository<Section>
    {
        Task<IReadOnlyList<Section>> GetByEventIdAsync( Guid eventId,CancellationToken cancellationToken = default);
    }
}
