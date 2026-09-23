
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Abstractions.Persistence
{
    public interface IFeatureRepository : IRepository<Feature>
    {
        Task<IReadOnlyList<Feature>> GetActiveAsync(CancellationToken cancellationToken = default);
    }
}
