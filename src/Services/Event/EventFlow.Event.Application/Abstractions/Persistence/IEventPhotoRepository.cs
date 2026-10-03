using EventFlow.Contracts.Common;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IEventPhotoRepository : IRepository<EventPhoto>
    {
        Task<IReadOnlyList<EventPhoto>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EventPhoto>> GetVisibleByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EventPhoto>> GetByPhotographerIdAsync(Guid photographerId, CancellationToken cancellationToken = default);
        Task<EventPhoto?> GetByIdWithEventAsync(Guid photoId, CancellationToken cancellationToken = default);
        Task<PaginatedResponse<EventPhoto>> GetPagedByEventIdAsync(Guid eventId, int page, int pageSize, bool visibleOnly, CancellationToken cancellationToken = default);
    }
}