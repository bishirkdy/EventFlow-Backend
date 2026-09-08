

using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class EventSettingsConfiguration: IEntityTypeConfiguration<EventSettings>
    {
        public void Configure(EntityTypeBuilder<EventSettings> builder)
        {
            // Table configuration
            builder.ToTable("EventSettings");

            // Primary key
            builder.HasKey(x => x.Id);

            // Event ID
            builder.Property(x => x.EventId)
                .IsRequired();

            // One settings record per event
            builder.HasIndex(x => x.EventId)
                .IsUnique();

            // Default language
            builder.Property(x => x.DefaultLanguage)
                .IsRequired()
                .HasMaxLength(10);

            // Audit fields
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);
        }
    }
}
