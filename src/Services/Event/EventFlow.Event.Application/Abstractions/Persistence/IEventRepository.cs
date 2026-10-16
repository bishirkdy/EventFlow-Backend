using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Persistence.ReadModels;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IEventRepository : IRepository<EventEntity>
    {
        // Get event with its images.
        Task<EventEntity?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken = default);

        // Get events by pagination
        Task<PaginatedResponse<EventEntity>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        
        // Get events created by a specific user
        Task<IReadOnlyList<EventEntity>> GetByCreatedByAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EventEntity>> GetByIdsAsync(IReadOnlyCollection<Guid> eventIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EventEntity>> GetPublishedUpcomingAsync(int take, CancellationToken cancellationToken = default);

        // Get all events by pagination for the admin console (without images).
        Task<PaginatedResponse<EventEntity>> GetAdminPagedAsync(int page, int pageSize, EventStatus? status, string? search, CancellationToken cancellationToken);

        // Get event counts grouped by status in a single query.
        Task<EventStatusCounts> GetStatusCountsAsync(CancellationToken cancellationToken);
    }
}
