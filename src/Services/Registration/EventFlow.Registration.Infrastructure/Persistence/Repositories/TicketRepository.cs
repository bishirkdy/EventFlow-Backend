using EventFlow.Registration.Application.Abstractions.Persistence;


namespace EventFlow.Registration.Infrastructure.Persistence.Repositories;

public sealed class TicketRepository(RegistrationDbContext db)
    : ITicketRepository
{
    public Task<Ticket?> GetForRegistrationAsync(Guid eventId, Guid registrationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return db.Tickets
            .AsNoTracking()
            .Include(x => x.Registration)
            .Include(x => x.Participant)
            .SingleOrDefaultAsync(
                x =>
                    x.RegistrationId == registrationId &&
                    x.Registration.EventId == eventId &&
                    x.Registration.UserId == userId,
                cancellationToken);
    }

    public Task<Ticket?> GetByQrCodeAsync(
        Guid eventId,
        string qrCodeValue,
        CancellationToken cancellationToken = default)
    {
        return db.Tickets
            .AsNoTracking()
            .Include(x => x.Registration)
            .Include(x => x.Participant)
            .SingleOrDefaultAsync(
                x =>
                    x.Registration.EventId == eventId &&
                    x.QrCodeValue == qrCodeValue,
                cancellationToken);
    }

    public Task<Ticket?> GetByIdAsync(
        Guid eventId,
        Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        return db.Tickets
            .Include(x => x.Registration)
            .Include(x => x.Participant)
            .SingleOrDefaultAsync(
                x =>
                    x.Id == ticketId &&
                    x.Registration.EventId == eventId,
                cancellationToken);
    }
}
