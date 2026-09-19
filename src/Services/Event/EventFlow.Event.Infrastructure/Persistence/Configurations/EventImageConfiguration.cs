using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class EventImageConfiguration : IEntityTypeConfiguration<EventImage>
    {
        public void Configure(EntityTypeBuilder<EventImage> builder)
        {
            builder.ToTable("EventImage");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventId)
                .IsRequired();

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.StorageKey)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SizeBytes)
                .IsRequired();

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasIndex(x => x.EventId);

            builder.HasIndex(x => new
            {
                x.EventId,
                x.DisplayOrder
            })
            .IsUnique();

            builder.HasOne(x => x.Event)
    .WithMany(x => x.Images)
    .HasForeignKey(x => x.EventId)
    .OnDelete(DeleteBehavior.Cascade);
            // EventImage -> Event relationship is configured
            // only in EventConfiguration.
        }
    }
}