namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class RegistrationFormFieldConfiguration : IEntityTypeConfiguration<RegistrationFormField>
{
    public void Configure(EntityTypeBuilder<RegistrationFormField> b)
    {
        b.ToTable("RegistrationFormFields");
        b.HasKey(x => x.Id);
        b.Property(x => x.FieldKey).HasMaxLength(100).IsRequired();
        b.Property(x => x.Label).HasMaxLength(250).IsRequired();
        b.Property(x => x.FieldType).HasConversion<int>();
        b.Property(x => x.OptionsJson).HasColumnType("jsonb");
        b.Property(x => x.ValidationJson).HasColumnType("jsonb");
        b.HasIndex(x => new
        {
            x.RegistrationFormId,
            x.FieldKey
        }
        ).IsUnique();

        b.HasMany(x => x.Answers)
            .WithOne(x => x.RegistrationFormField)
            .HasForeignKey(x => x.RegistrationFormFieldId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
