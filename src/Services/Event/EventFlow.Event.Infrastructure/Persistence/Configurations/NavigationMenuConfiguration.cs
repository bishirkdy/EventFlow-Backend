

using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class NavigationMenuConfiguration : IEntityTypeConfiguration<NavigationMenu>
    {
        public void Configure(EntityTypeBuilder<NavigationMenu> builder)
        {
            builder.ToTable("NavigationMenus");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventId)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Location)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasIndex(x => x.EventId);

            builder.HasOne<EventEntity>()
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
