

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class FeatureRepository(EventCoreDbContext context): GenericRepository<Feature>(context), IFeatureRepository
    {
        public async Task<IReadOnlyList<Feature>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Features
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
