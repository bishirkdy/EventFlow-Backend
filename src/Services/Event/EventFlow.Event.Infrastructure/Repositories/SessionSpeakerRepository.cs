using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories;

public sealed class SessionSpeakerRepository(EventCoreDbContext context) : GenericRepository<SessionSpeaker>(context), ISessionSpeakerRepository
{
    public Task<bool> ExistsAsync(Guid sessionId, Guid speakerId, CancellationToken cancellationToken = default) =>
        Context.Set<SessionSpeaker>().AnyAsync(x => x.SessionId == sessionId && x.SpeakerId == speakerId, cancellationToken);

    public async Task<IReadOnlyList<SessionSpeaker>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        await Context.Set<SessionSpeaker>().AsNoTracking().Include(x => x.Speaker).Where(x => x.SessionId == sessionId).OrderBy(x => x.Speaker.Name).ToListAsync(cancellationToken);
}
