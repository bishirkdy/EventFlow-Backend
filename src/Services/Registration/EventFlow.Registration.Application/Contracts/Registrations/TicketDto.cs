namespace EventFlow.Registration.Application.Contracts.Registrations;

public sealed class TicketDto
{
    public Guid Id { get; set; }

    public Guid RegistrationId { get; set; }

    public Guid ParticipantId { get; set; }

    public Guid ParticipantUserId { get; set; }

    public string TicketNumber { get; set; } = "";

    public string QrCodeValue { get; set; } = "";

    public DateTime IssuedAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public bool IsActive { get; set; }
}
