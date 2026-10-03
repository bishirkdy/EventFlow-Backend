using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Infrastructure.Persistence;

public sealed class RegistrationDbContext(
    DbContextOptions<RegistrationDbContext> options)
    : DbContext(options)
{
    public DbSet<RegistrationEntity> Registrations => Set<RegistrationEntity>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<RegistrationForm> RegistrationForms => Set<RegistrationForm>();
    public DbSet<RegistrationFormField> RegistrationFormFields => Set<RegistrationFormField>();
    public DbSet<RegistrationAnswer> RegistrationAnswers => Set<RegistrationAnswer>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<CertificateSettings> CertificateSettings => Set<CertificateSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(RegistrationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
