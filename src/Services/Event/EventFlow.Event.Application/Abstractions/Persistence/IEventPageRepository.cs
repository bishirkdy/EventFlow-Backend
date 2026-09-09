
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IEventPageRepository : IRepository<EventPage>
    {
        Task<IReadOnlyList<EventPage>> GetByEventIdAsync(Guid eventId,CancellationToken cancellationToken = default);

        Task<EventPage?> GetBySlugAsync(Guid eventId,string slug,CancellationToken cancellationToken = default);
    }
}
