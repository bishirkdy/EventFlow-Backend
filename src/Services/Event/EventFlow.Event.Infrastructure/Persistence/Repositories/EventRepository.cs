using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Persistence.ReadModels;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories;

public sealed class EventRepository : GenericRepository<EventEntity>, IEventRepository
{
    private readonly EventCoreDbContext _context;

    public EventRepository(EventCoreDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<EventEntity?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EventEntities
            .AsNoTracking()
            .Include(x => x.EventType)
            .Include(x => x.Images.OrderBy(image => image.DisplayOrder))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PaginatedResponse<EventEntity>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.EventEntities
            .AsNoTracking()
            .Include(x => x.EventType)
            .Include(x => x.Images.OrderBy(image => image.DisplayOrder))
            .AsSplitQuery()
            .OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedResponse<EventEntity>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<EventEntity>> GetByCreatedByAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.EventEntities
            .AsNoTracking()
            .Include(x => x.EventType)
            .Include(x => x.Images.OrderBy(image => image.DisplayOrder))
            .AsSplitQuery()
            .Where(x => x.CreatedBy == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventEntity>> GetByIdsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken = default)
    {
        if (eventIds.Count == 0)
        {
            return [];
        }

        return await _context.EventEntities
            .AsNoTracking()
            .Include(x => x.EventType)
            .Include(x => x.Images.OrderBy(image => image.DisplayOrder))
            .AsSplitQuery()
            .Where(x => eventIds.Contains(x.Id))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventEntity>> GetPublishedUpcomingAsync(int take, CancellationToken cancellationToken = default)
    {
        return await _context.EventEntities
            .AsNoTracking()
            .Include(x => x.EventType)
            .Include(x => x.Images.OrderBy(image => image.DisplayOrder))
            .AsSplitQuery()
            .Where(x => x.Status == EventFlow.Event.Domain.Enums.EventStatus.Published && x.EndDate >= DateTime.UtcNow)
            .OrderBy(x => x.StartDate)
            .Take(Math.Clamp(take, 1, 12))
            .ToListAsync(cancellationToken);
    }

    // Get all events by pagination for the admin console (without images).
    public async Task<PaginatedResponse<EventEntity>> GetAdminPagedAsync(
        int page,
        int pageSize,
        EventStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = _context.EventEntities
            .AsNoTracking()
            .Include(x => x.EventType)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{term}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<EventEntity>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    // Get event counts grouped by status in a single query.
    public async Task<EventStatusCounts> GetStatusCountsAsync(CancellationToken cancellationToken)
    {
        var counts = await _context.EventEntities
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(group => new EventStatusCounts(
                group.Count(),
                group.Count(x => x.Status == EventStatus.Published),
                group.Count(x => x.Status == EventStatus.Draft),
                group.Count(x => x.Status == EventStatus.Cancelled)))
            .FirstOrDefaultAsync(cancellationToken);

        return counts ?? new EventStatusCounts(0, 0, 0, 0);
    }
}
