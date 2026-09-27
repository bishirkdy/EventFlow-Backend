using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class NavigationItemConfiguration : IEntityTypeConfiguration<NavigationItem>
    {
        public void Configure(EntityTypeBuilder<NavigationItem> builder)
        {
            builder.ToTable("NavigationItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventId)
                .IsRequired();

            builder.Property(x => x.Label)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.PageId)
                .IsRequired();

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.Property(x => x.IsVisible)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasIndex(x => new
            {
                x.EventId,
                x.DisplayOrder
            });

            builder.HasOne<EventEntity>()
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<EventPage>()
                .WithMany()
                .HasForeignKey(x => x.PageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
