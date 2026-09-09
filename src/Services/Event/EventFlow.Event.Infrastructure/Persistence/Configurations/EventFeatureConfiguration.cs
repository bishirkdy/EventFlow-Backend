

using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class EventFeatureConfiguration: IEntityTypeConfiguration<EventFeature>
    {
        public void Configure(EntityTypeBuilder<EventFeature> builder)
        {
            // Table
            builder.ToTable("EventFeatures");

            // Primary key
            builder.HasKey(x => x.Id);

            // Event
            builder.Property(x => x.EventId)
                .IsRequired();

            // Feature
            builder.Property(x => x.FeatureId)
                .IsRequired();

            // Status
            builder.Property(x => x.IsEnabled)
                .IsRequired();

            // Audit fields
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            // One feature can be enabled only once per event
            builder.HasIndex(x => new
            {
                x.EventId,
                x.FeatureId
            })
            .IsUnique();
        }
    }
}
