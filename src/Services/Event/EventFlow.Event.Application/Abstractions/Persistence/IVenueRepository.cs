

using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IVenueRepository : IRepository<Venue>
    {
        Task<IReadOnlyList<Venue>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
