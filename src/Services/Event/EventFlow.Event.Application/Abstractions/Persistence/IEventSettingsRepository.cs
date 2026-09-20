

using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IEventSettingsRepository : IRepository<EventSettings>
    {
        // Get settings for an event
        Task<EventSettings?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
