using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations;

public sealed class SessionSpeakerConfiguration : IEntityTypeConfiguration<SessionSpeaker>
{
    public void Configure(EntityTypeBuilder<SessionSpeaker> builder)
    {
        builder.ToTable("SessionSpeakers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SessionId).IsRequired();
        builder.Property(x => x.SpeakerId).IsRequired();
        builder.HasIndex(x => new { x.SessionId, x.SpeakerId }).IsUnique();
        builder.HasOne(x => x.Session).WithMany().HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Speaker).WithMany().HasForeignKey(x => x.SpeakerId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
    }
}
