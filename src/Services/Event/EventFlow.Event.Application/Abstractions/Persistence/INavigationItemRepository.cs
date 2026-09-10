
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface INavigationItemRepository: IRepository<NavigationItem>
    {
        Task<IReadOnlyList<NavigationItem>> GetByMenuIdAsync(Guid navigationMenuId,CancellationToken cancellationToken = default);

    }
}
