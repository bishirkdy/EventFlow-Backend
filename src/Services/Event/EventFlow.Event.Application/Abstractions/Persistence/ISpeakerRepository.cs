using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence;

public interface ISpeakerRepository : IRepository<Speaker>
{
    Task<IReadOnlyList<Speaker>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<Speaker?> GetByIdWithSessionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session>> GetSessionsAsync(Guid speakerId, CancellationToken cancellationToken = default);
}
