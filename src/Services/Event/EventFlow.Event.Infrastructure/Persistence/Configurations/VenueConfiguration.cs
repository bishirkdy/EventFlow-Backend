
using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class VenueConfiguration: IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            // Table
            builder.ToTable("Venues");

            // Primary key
            builder.HasKey(x => x.Id);

            // Event
            builder.Property(x => x.EventId)
                .IsRequired();

            // Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Description
            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            // Address
            builder.Property(x => x.Address)
                .HasMaxLength(500);

            // Capacity
            builder.Property(x => x.Capacity)
                .IsRequired();

            // Active status
            builder.Property(x => x.IsActive)
                .IsRequired();

            // Audit fields
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            // Event lookup
            builder.HasIndex(x => x.EventId);
        }
    }
}
