using Microsoft.EntityFrameworkCore;
using EventFlow.Registration.Domain.Entities;
using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Abstractions.Persistence;

public interface IRegistrationDbContext
{
    DbSet<RegistrationEntity> Registrations { get; }

    DbSet<Participant> Participants { get; }

    DbSet<RegistrationForm> RegistrationForms { get; }

    DbSet<RegistrationFormField> RegistrationFormFields { get; }

    DbSet<RegistrationAnswer> RegistrationAnswers { get; }

    DbSet<Ticket> Tickets { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}