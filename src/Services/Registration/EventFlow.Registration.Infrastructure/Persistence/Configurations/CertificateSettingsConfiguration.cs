namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class CertificateSettingsConfiguration : IEntityTypeConfiguration<CertificateSettings>
{
    public void Configure(EntityTypeBuilder<CertificateSettings> b)
    {
        b.ToTable("CertificateSettings");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.EventId).IsUnique();
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Subtitle).HasMaxLength(500).IsRequired();
        b.Property(x => x.SignatoryName).HasMaxLength(200);
        b.Property(x => x.SignatoryTitle).HasMaxLength(200);
        b.Property(x => x.ThemeColor).HasMaxLength(9).IsRequired();
    }
}
