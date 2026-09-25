using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace EventFlow.Event.Infrastructure.Persistence
{
    public sealed class EventCoreDbContext : DbContext
    {
        public EventCoreDbContext(DbContextOptions<EventCoreDbContext> options): base(options)
        {
        }

        public DbSet<EventEntity> EventEntities => Set<EventEntity>();
        public DbSet<EventImage> EventImages => Set<EventImage>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<Speaker> Speakers => Set<Speaker>();
        public DbSet<Sponsor> Sponsors => Set<Sponsor>();
        public DbSet<SessionSpeaker> SessionSpeakers => Set<SessionSpeaker>();
        public DbSet<EventFeature> EventFeatures => Set<EventFeature>();
        public DbSet<Feature> Features => Set<Feature>();

        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<EventPage> EventPages => Set<EventPage>();
        public DbSet<PageSection> PageSections => Set<PageSection>();
        public DbSet<NavigationMenu> NavigationMenus => Set<NavigationMenu>();
        public DbSet<NavigationItem> NavigationItems => Set<NavigationItem>();
        public DbSet<EventType> EventTypes => Set<EventType>();
        public DbSet<EventTypeFeature> EventTypeFeatures => Set<EventTypeFeature>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(EventCoreDbContext).Assembly);
        }
    }
}
