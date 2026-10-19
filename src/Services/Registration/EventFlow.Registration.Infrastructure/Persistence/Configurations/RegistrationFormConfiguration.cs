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
        b.Property(x => x.ScopeType).HasConversion<int>();
        b.HasIndex(x => new { x.EventId, x.ScopeType, x.ScopeId }).IsUnique();
        b.HasIndex(x => x.EventId)
            .IsUnique()
            .HasFilter("\"ScopeId\" IS NULL");

        b.HasMany(x => x.Fields).WithOne(x => x.RegistrationForm)
            .HasForeignKey(x => x.RegistrationFormId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt);
    }
}
