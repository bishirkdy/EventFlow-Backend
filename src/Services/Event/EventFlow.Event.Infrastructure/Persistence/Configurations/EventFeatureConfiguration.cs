
using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations;

public sealed class EventFeatureConfiguration : IEntityTypeConfiguration<EventFeature>
{
    public void Configure(EntityTypeBuilder<EventFeature> builder)
    {
        builder.ToTable("EventFeatures");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventId)
            .IsRequired();

        builder.Property(x => x.FeatureId)
            .IsRequired();

        builder.Property(x => x.IsEnabled)
            .IsRequired();

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

        builder.HasOne(x => x.Feature)
            .WithMany()
            .HasForeignKey(x => x.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
