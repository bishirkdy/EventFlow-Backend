using EventFlow.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Identity.Infrastructure.Persistence.Configurations;

public class EventInvitationConfiguration : IEntityTypeConfiguration<EventInvitation>
{
    public void Configure(EntityTypeBuilder<EventInvitation> builder)
    {
        builder.ToTable("EventInvitations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.ScopeType)
            .HasConversion<int>();

        builder.Property(x => x.ScopeId)
            .IsRequired(false);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.AcceptedAt)
            .IsRequired(false);

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasIndex(x => new { x.EventId, x.Type, x.Email })
            .IsUnique()
            .HasFilter("\"Status\" = 0"); // Only one pending invitation per type and email per event

        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.AcceptedByUser)
            .WithMany()
            .HasForeignKey(x => x.AcceptedByUserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
