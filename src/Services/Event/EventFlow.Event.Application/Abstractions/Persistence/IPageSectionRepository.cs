using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence;

public interface IPageSectionRepository : IRepository<PageSection>
{
    Task<IReadOnlyList<PageSection>> GetByPageIdAsync(
        Guid pageId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PageSection>> GetByPageIdAsync(
        Guid pageId,
        bool includeUnpublished,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PageSection>> GetByEventIdAsync(Guid eventId, bool includeUnpublished = false, CancellationToken cancellationToken = default);
}
