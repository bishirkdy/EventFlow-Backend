using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Abstractions.Persistence;

public interface ITicketRepository
{
    Task<Ticket?> GetForRegistrationAsync(
        Guid eventId,
        Guid registrationId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Ticket?> GetByQrCodeAsync(
        Guid eventId,
        string qrCodeValue,
        CancellationToken cancellationToken = default);

    Task<Ticket?> GetByIdAsync(
        Guid eventId,
        Guid ticketId,
        CancellationToken cancellationToken = default);
}
