namespace EventFlow.Operations.Application.Abstractions;

public interface IRegistrationClient
{
    Task<VerifiedTicket?> VerifyTicketAsync(Guid eventId, string qrCode, string? bearerToken, CancellationToken cancellationToken = default);
}
