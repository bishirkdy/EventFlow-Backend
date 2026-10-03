namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> b)
    {
        b.ToTable("Certificates");
        b.HasKey(x => x.Id);
        b.Property(x => x.CertificateNumber).HasMaxLength(60).IsRequired();
        b.HasIndex(x => x.CertificateNumber).IsUnique();
        b.Property(x => x.ParticipantName).HasMaxLength(200).IsRequired();
        b.Property(x => x.ParticipantEmail).HasMaxLength(320).IsRequired();
        b.Property(x => x.EventName).HasMaxLength(300).IsRequired();
        b.Property(x => x.DocumentFileName).HasMaxLength(260).IsRequired();
        b.Property(x => x.Status).HasConversion<int>();

        b.HasIndex(x => new { x.EventId, x.RegistrationId }).IsUnique();
        b.HasIndex(x => x.EventId);
        b.HasIndex(x => x.UserId);
    }
}
