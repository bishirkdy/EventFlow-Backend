using EventFlow.Event.Application.Common.Models;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IEventRepository : IRepository<EventEntity>
    {
        //Get events by pagination
        Task<PaginatedResult<EventEntity>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        
        // Get events created by a specific user
        Task<IReadOnlyList<EventEntity>> GetByCreatedByAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
