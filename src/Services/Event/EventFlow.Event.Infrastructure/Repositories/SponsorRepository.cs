using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories;

public sealed class SponsorRepository(EventCoreDbContext context) : GenericRepository<Sponsor>(context), ISponsorRepository
{
    public async Task<IReadOnlyList<Sponsor>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        await Context.Set<Sponsor>().AsNoTracking().Where(x => x.EventId == eventId).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync(cancellationToken);
}
