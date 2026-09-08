using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class EventConfiguration : IEntityTypeConfiguration<EventEntity>
    {
        public void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            // Table configuration.
            builder.ToTable("Events");

            // Primary key.
            builder.HasKey(x => x.Id);

            // Event name.
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Event description.
            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            // Event type.
            builder.Property(x => x.EventType)
                .IsRequired()
                .HasMaxLength(100);

            // Event subtype.
            builder.Property(x => x.SubType)
                .HasMaxLength(100);

            // Event dates.
            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            // IANA time zone ID, for example:
            // Asia/Kolkata
            // Europe/London
            builder.Property(x => x.TimeZone)
                .IsRequired()
                .HasMaxLength(100);

            // Event status enum.
            builder.Property(x => x.Status)
                .IsRequired();

            // Identity user's ID who created the event.
            builder.Property(x => x.CreatedBy)
                .IsRequired();

            // Audit fields.
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);
        }
    }
}
