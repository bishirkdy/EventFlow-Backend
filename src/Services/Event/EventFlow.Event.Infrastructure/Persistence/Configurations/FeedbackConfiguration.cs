using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedbacks");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.EventId).IsRequired();
            builder.Property(x => x.ParticipantUserId).IsRequired();
            builder.Property(x => x.TargetType).IsRequired();
            builder.Property(x => x.TargetId);
            builder.Property(x => x.Rating).IsRequired();
            builder.Property(x => x.Comment).HasMaxLength(2000);
            builder.Property(x => x.SubmittedAtUtc).IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasIndex(x => x.EventId);
            builder.HasIndex(x => new { x.EventId, x.TargetType });

            // One feedback entry per participant, per target.
            builder.HasIndex(x => new { x.EventId, x.ParticipantUserId, x.TargetType, x.TargetId })
                .IsUnique();
        }
    }
}
