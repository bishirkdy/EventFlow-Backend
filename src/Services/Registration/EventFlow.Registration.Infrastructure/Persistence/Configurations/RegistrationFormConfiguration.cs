namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class RegistrationFormConfiguration : IEntityTypeConfiguration<RegistrationForm>
{
    public void Configure(EntityTypeBuilder<RegistrationForm> b)
    {
        b.ToTable("RegistrationForms");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.CapacityMode).HasConversion<int>();
        b.HasIndex(x => x.EventId).IsUnique();

        b.HasMany(x => x.Fields).WithOne(x => x.RegistrationForm)
            .HasForeignKey(x => x.RegistrationFormId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
