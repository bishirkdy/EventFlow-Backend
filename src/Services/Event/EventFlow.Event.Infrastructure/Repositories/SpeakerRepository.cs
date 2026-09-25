using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories;

public sealed class SpeakerRepository(EventCoreDbContext context) : GenericRepository<Speaker>(context), ISpeakerRepository
{
    public async Task<IReadOnlyList<Speaker>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        await Context.Set<Speaker>().AsNoTracking().Where(x => x.EventId == eventId).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task<Speaker?> GetByIdWithSessionsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Set<Speaker>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Session>> GetSessionsAsync(Guid speakerId, CancellationToken cancellationToken = default) =>
        await Context.Set<SessionSpeaker>().AsNoTracking().Where(x => x.SpeakerId == speakerId).Include(x => x.Session).OrderBy(x => x.Session.StartTimeUtc).Select(x => x.Session).ToListAsync(cancellationToken);
}
