using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence;

public interface ISessionSpeakerRepository : IRepository<SessionSpeaker>
{
    Task<bool> ExistsAsync(Guid sessionId, Guid speakerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SessionSpeaker>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
