
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface INavigationItemRepository: IRepository<NavigationItem>
    {
        Task<IReadOnlyList<NavigationItem>> GetByEventIdAsync(Guid eventId,CancellationToken cancellationToken);
        Task<NavigationItem?> GetByPageIdAsync(Guid pageId, CancellationToken cancellationToken);
    }
}
