

using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IEventTypeRepository : IRepository<EventType>
    {

        Task<EventType?> GetByCodeAsync(EventTypeCode code,CancellationToken cancellationToken);

        Task<IReadOnlyList<EventType>> GetActiveAsync(CancellationToken cancellationToken);
    }
}
