

using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface INavigationMenuRepository : IRepository<NavigationMenu>
    {
        Task<IReadOnlyList<NavigationMenu>> GetByEventIdAsync(Guid eventId,CancellationToken cancellationToken = default);
    }
}
