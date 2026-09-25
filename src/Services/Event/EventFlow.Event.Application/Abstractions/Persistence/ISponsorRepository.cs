using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence;

public interface ISponsorRepository : IRepository<Sponsor>
{
    Task<IReadOnlyList<Sponsor>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
}
