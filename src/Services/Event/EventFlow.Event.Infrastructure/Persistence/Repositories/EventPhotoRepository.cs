using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories;

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

    public async Task<IReadOnlyList<EventPhoto>> GetByIdsAsync(IReadOnlyList<Guid> photoIds, CancellationToken cancellationToken = default)
    {
        if (photoIds.Count == 0)
        {
            return [];
        }

        return await DbSet
            .Where(x => photoIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<EventPhoto>> GetPagedByEventIdAsync(Guid eventId, int page, int pageSize, bool? visibleOnly, Guid? photographerId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(x => x.EventId == eventId);

        // Apply the filter when true OR false.
        // null means return all photos.
        if (visibleOnly.HasValue)
        {
            query = query.Where(x => x.IsVisible == visibleOnly.Value);
        }

        if (photographerId.HasValue)
        {
            query = query.Where(x => x.PhotographerId == photographerId.Value);
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
