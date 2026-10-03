using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories;

public sealed class EventTypeRepository : GenericRepository<EventType>, IEventTypeRepository
{
    public EventTypeRepository(EventCoreDbContext context) : base(context)
    {
    }

    public async Task<EventType?> GetByCodeAsync(
        EventTypeCode code, CancellationToken cancellationToken)
    {
        return await Context.EventTypes
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<EventType>> GetActiveAsync(
        CancellationToken cancellationToken)
    {
        return await Context.EventTypes
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
