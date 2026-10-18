using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations;

public sealed class EventPageConfiguration : IEntityTypeConfiguration<EventPage>
{
    public void Configure(EntityTypeBuilder<EventPage> builder)
    {
        builder.ToTable("EventPages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PageType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.IsPublished)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        // This allows different events to have the same slug
        builder.HasIndex(x => new
        {
            x.EventId,
            x.Slug
        }).IsUnique();

        builder.HasIndex(x => x.EventId);
    }
}
