using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public class EventPhotoConfiguration : IEntityTypeConfiguration<EventPhoto>
    {
        public void Configure(EntityTypeBuilder<EventPhoto> builder)
        {
            builder.ToTable("EventPhotos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.PublicId)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ThumbnailUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.IsVisible)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.UploadedAt)
                .IsRequired();

            builder.Property(x => x.ApprovedAt)
                .IsRequired(false);

            builder.Property(x => x.FaceEmbeddingsJson)
                .HasMaxLength(10000);

            builder.HasIndex(x => x.EventId);
            builder.HasIndex(x => x.PhotographerId);
            builder.HasIndex(x => x.IsVisible);
            builder.HasIndex(x => new { x.EventId, x.IsVisible });

            // Foreign key relationships without navigation properties
            builder.HasIndex(x => x.ApprovedBy);
        }
    }
}