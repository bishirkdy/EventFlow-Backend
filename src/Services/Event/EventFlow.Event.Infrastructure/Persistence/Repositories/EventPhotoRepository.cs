using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using EventFlow.Contracts.Common;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories
{
    public sealed class EventPhotoRepository(EventCoreDbContext context) : GenericRepository<EventPhoto>(context), IEventPhotoRepository
    {
        public async Task<IReadOnlyList<EventPhoto>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.EventId == eventId)
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<EventPhoto>> GetVisibleByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.EventId == eventId && x.IsVisible)
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<EventPhoto>> GetByPhotographerIdAsync(Guid photographerId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.PhotographerId == photographerId)
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<EventPhoto?> GetByIdWithEventAsync(Guid photoId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(x => x.Id == photoId, cancellationToken);
        }

        public async Task<PaginatedResponse<EventPhoto>> GetPagedByEventIdAsync(Guid eventId, int page, int pageSize, bool visibleOnly, CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(x => x.EventId == eventId);

            if (visibleOnly)
            {
                query = query.Where(x => x.IsVisible);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResponse<EventPhoto>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}