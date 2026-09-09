

using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<IReadOnlyList<Session>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
