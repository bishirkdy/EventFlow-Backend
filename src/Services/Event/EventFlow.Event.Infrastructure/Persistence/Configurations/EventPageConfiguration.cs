using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class EventPageConfiguration: IEntityTypeConfiguration<EventPage>
    {
        public void Configure(EntityTypeBuilder<EventPage> builder)
        {
            // Table
            builder.ToTable("EventPages");

            // Primary key
            builder.HasKey(x => x.Id);

            // Event
            builder.Property(x => x.EventId)
                .IsRequired();

            // Page name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Page slug
            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(200);

            // Page type
            builder.Property(x => x.PageType)
                .IsRequired()
                .HasMaxLength(100);

            // Display order
            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            // Published status
            builder.Property(x => x.IsPublished)
                .IsRequired();

            // Audit fields
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            // Index - This allows different events to have the same slug
            builder.HasIndex(x => new
            {
                x.EventId,
                x.Slug
            }).IsUnique();

            // Event lookup
            builder.HasIndex(x => x.EventId);
        }
    }
}
