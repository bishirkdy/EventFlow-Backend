

using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class PageSectionConfiguration: IEntityTypeConfiguration<PageSection>
    {
        public void Configure(EntityTypeBuilder<PageSection> builder)
        {
            builder.ToTable("PageSections");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PageId)
                .IsRequired();

            builder.Property(x => x.SectionType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Title)
                .HasMaxLength(200);

            builder.Property(x => x.Content)
                .HasMaxLength(10000);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.Property(x => x.IsVisible)
                .IsRequired();

            builder.Property(x => x.Configuration)
                .HasColumnType("jsonb");

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasIndex(x => new
            {
                x.PageId,
                x.DisplayOrder
            });

            builder.HasOne<EventPage>()
                .WithMany()
                .HasForeignKey(x => x.PageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
