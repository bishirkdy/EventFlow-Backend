
namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class RegistrationAnswerConfiguration : IEntityTypeConfiguration<RegistrationAnswer>
{
    public void Configure(EntityTypeBuilder<RegistrationAnswer> b)
    {
        b.ToTable("RegistrationAnswers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Value).HasMaxLength(4000).IsRequired();
        b.HasIndex(x => new
        {
            x.RegistrationId,
            x.RegistrationFormFieldId
        }
    ).IsUnique();
    }
}
