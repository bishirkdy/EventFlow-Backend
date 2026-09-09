
using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class SessionConfiguration: IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("Sessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventId)
                .IsRequired();

            builder.Property(x => x.SectionId)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.SessionType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Capacity);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasIndex(x => x.EventId);
            builder.HasIndex(x => x.SectionId);
        }
    }
}
