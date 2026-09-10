using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class PageSectionRepository(EventCoreDbContext context): GenericRepository<PageSection>(context), IPageSectionRepository
    {
        public async Task<IReadOnlyList<PageSection>> GetByPageIdAsync(Guid pageId,CancellationToken cancellationToken = default)
        {
            // Get sections belonging to the page
            return await Context.PageSections
                .Where(x => x.PageId == pageId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);
        }
    }
}
