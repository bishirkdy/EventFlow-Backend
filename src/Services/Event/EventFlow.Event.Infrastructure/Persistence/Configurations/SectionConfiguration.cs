

using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            // Table
            builder.ToTable("Sections");

            // Primary key
            builder.HasKey(x => x.Id);

            // Event relationship
            builder.Property(x => x.EventId)
                .IsRequired();

            // Section name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Description
            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            // Display order
            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            // Active status
            builder.Property(x => x.IsActive)
                .IsRequired();

            // Audit fields
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            // Index for event sections
            builder.HasIndex(x => x.EventId);
        }
    }
}
