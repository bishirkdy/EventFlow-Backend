using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories;

public sealed class EventSettingsRepository : GenericRepository<EventSettings>, IEventSettingsRepository
{
    public EventSettingsRepository(EventCoreDbContext context) : base(context)
    {
    }

    public async Task<EventSettings?> GetByEventIdAsync(
        Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EventSettings>()
            .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
    }
}
