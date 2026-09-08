using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Infrastructure.Persistence;


namespace EventFlow.Event.Infrastructure.Repositories
{
    public sealed class UnitOfWork(EventCoreDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
