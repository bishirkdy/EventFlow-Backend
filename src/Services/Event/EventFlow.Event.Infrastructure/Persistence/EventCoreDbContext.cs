using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace EventFlow.Event.Infrastructure.Persistence
{
    public sealed class EventCoreDbContext : DbContext
    {
        public EventCoreDbContext(DbContextOptions<EventCoreDbContext> options): base(options)
        {
        }

        public DbSet<EventEntity> EventEntities => Set<EventEntity>();
        public DbSet<Section> Sections => Set<Section>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(EventCoreDbContext).Assembly);
        }
    }
}
