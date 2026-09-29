namespace EventFlow.Registration.Domain.Entities;

public sealed class Ticket
{
    public Guid Id { get; set; }

    public Guid RegistrationId { get; set; }

    public Guid ParticipantId { get; set; }

    public string TicketNumber { get; set; } = "";

    public string QrCodeValue { get; set; } = "";

    public DateTime IssuedAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public bool IsActive { get; set; } = true;

    public Registration Registration { get; set; } = null!;

    public Participant Participant { get; set; } = null!;
}