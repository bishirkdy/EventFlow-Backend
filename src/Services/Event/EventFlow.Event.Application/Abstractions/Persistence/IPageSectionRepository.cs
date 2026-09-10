
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IPageSectionRepository : IRepository<PageSection>
    {
        Task<IReadOnlyList<PageSection>> GetByPageIdAsync(Guid pageId,CancellationToken cancellationToken = default);
    }
}
