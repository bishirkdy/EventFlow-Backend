using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Common;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace EventFlow.Event.Infrastructure.Repositories
{
    //Generic crud to reduce boilerplate
    public class GenericRepository<T>(EventCoreDbContext context) : IRepository<T> where T : Entity
    {
        protected readonly EventCoreDbContext Context = context;
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync([id], cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity,cancellationToken);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}
