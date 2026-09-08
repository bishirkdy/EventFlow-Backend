
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class EventSettingsRepository(EventCoreDbContext context) : IEventSettingsRepository
    {
        // Get settings by event ID
        public async Task<EventSettings?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await context.Set<EventSettings>()
                .FirstOrDefaultAsync(x => x.EventId == eventId,cancellationToken);
        }
    }
}
