
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class SectionRepository(EventCoreDbContext context) : GenericRepository<Section>(context), ISectionRepository
    {
        public async Task<IReadOnlyList<Section>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await Context.Set<Section>()
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);
        }
    }
}
