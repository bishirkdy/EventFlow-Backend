using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories;

public sealed class PageSectionRepository(EventCoreDbContext context)
    : GenericRepository<PageSection>(context), IPageSectionRepository
{
    public Task<IReadOnlyList<PageSection>> GetByPageIdAsync(
        Guid pageId,
        CancellationToken cancellationToken = default)
    {
        return GetByPageIdAsync(pageId, includeUnpublished: false, cancellationToken);
    }

    public async Task<IReadOnlyList<PageSection>> GetByPageIdAsync(
        Guid pageId,
        bool includeUnpublished,
        CancellationToken cancellationToken = default)
    {
        var query = Context.PageSections.Where(x => x.PageId == pageId);

        if (!includeUnpublished)
        {
            query = query.Where(section =>
                Context.EventPages.Any(page =>
                    page.Id == section.PageId &&
                    page.IsPublished &&
                    Context.EventEntities.Any(eventEntity =>
                        eventEntity.Id == page.EventId &&
                        eventEntity.Status == EventFlow.Event.Domain.Enums.EventStatus.Published)));
        }

        return await query.OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PageSection>> GetByEventIdAsync(
        Guid eventId,
        bool includeUnpublished = false,
        CancellationToken cancellationToken = default)
    {
        var query = Context.PageSections.Where(section =>
            Context.EventPages.Any(page => page.Id == section.PageId && page.EventId == eventId));

        if (!includeUnpublished)
        {
            query = query.Where(section =>
                Context.EventPages.Any(page => page.Id == section.PageId && page.IsPublished) &&
                Context.EventEntities.Any(eventEntity =>
                    eventEntity.Id == eventId &&
                    eventEntity.Status == EventFlow.Event.Domain.Enums.EventStatus.Published));
        }

        return await query
            .OrderBy(section => section.PageId)
            .ThenBy(section => section.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
