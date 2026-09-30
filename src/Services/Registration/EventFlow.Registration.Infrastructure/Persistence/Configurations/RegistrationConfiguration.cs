using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class RegistrationConfiguration : IEntityTypeConfiguration<RegistrationEntity>
{
    public void Configure(EntityTypeBuilder<RegistrationEntity> b)
    {
        b.ToTable("Registrations");
        b.HasKey(x => x.Id);
        b.Property(x => x.RegistrationNumber).HasMaxLength(50).IsRequired();
        b.HasIndex(x => x.RegistrationNumber).IsUnique();
        b.Property(x => x.Status).HasConversion<int>();
        b.HasIndex(x => new
        {
            x.EventId,
            x.UserId
        })
        .HasFilter("\"Status\" IN (1, 2, 5)")
        .IsUnique();

        b.HasIndex(x => new
        {
            x.EventId,
            x.WaitlistPosition
        })
        .HasFilter("\"Status\" = 5 AND \"WaitlistPosition\" IS NOT NULL")
        .IsUnique();

        b.HasOne(x => x.Participant).WithOne(x => x.Registration)
            .HasForeignKey<Participant>(x => x.RegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Answers)
            .WithOne(x => x.Registration)
            .HasForeignKey(x => x.RegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Ticket).WithOne(x => x.Registration)
            .HasForeignKey<Ticket>(x => x.RegistrationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
