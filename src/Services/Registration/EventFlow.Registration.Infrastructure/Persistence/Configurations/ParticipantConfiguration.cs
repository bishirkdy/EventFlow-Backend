namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> b)
    {
        b.ToTable("Participants");

        b.HasKey(x => x.Id);
        b.Property(x => x.ParticipantNumber)
            .HasMaxLength(50).IsRequired();

        b.HasIndex(x => x.ParticipantNumber)
            .IsUnique();

        b.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.Email).
            HasMaxLength(320)
            .IsRequired();

        b.Property(x => x.Phone)
            .HasMaxLength(30);

        b.Property(x => x.Organization)
            .HasMaxLength(200);

        b.Property(x => x.Designation)
            .HasMaxLength(150);

        b.Property(x => x.Status)
            .HasConversion<int>();
    }
}
