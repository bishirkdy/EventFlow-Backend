using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class SessionRepository(EventCoreDbContext context) : GenericRepository<Session>(context) , ISessionRepository
    {
        public async Task<IReadOnlyList<Session>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await Context.Set<Session>()
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
