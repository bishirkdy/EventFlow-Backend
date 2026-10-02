namespace EventFlow.Operations.Application.Abstractions;

public sealed record VerifiedTicket(Guid RegistrationId, Guid ParticipantId, Guid ParticipantUserId, bool IsValid, string? Message);

public interface IRegistrationClient
{
    Task<VerifiedTicket?> VerifyTicketAsync(Guid eventId, string qrCode, string? bearerToken, CancellationToken cancellationToken = default);
}
