using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common.Models;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class EventRepository : GenericRepository<EventEntity>, IEventRepository
    {
        private readonly EventCoreDbContext _context;

        public EventRepository(EventCoreDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<EventEntity?> GetByIdWithImagesAsync(Guid id,CancellationToken cancellationToken = default)
        {
            return await _context.EventEntities
                .AsNoTracking()
                .Include(x => x.Images.OrderBy(image => image.DisplayOrder))
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<PaginatedResult<EventEntity>> GetPagedAsync(int page,int pageSize,CancellationToken cancellationToken)
        {
            var query = _context.EventEntities
                .AsNoTracking()
                .Include(x => x.Images.OrderBy(image => image.DisplayOrder))
                .AsSplitQuery()
                .OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<EventEntity>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IReadOnlyList<EventEntity>> GetByCreatedByAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.EventEntities
                .AsNoTracking()
                .Include(x => x.Images.OrderBy(image => image.DisplayOrder))
                .AsSplitQuery()
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
