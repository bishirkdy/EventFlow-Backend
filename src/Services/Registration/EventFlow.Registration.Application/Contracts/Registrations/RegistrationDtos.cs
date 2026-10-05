using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Contracts.Registrations;

public sealed class RegistrationDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public string RegistrationNumber { get; set; } = "";
    public RegistrationStatus Status { get; set; }
    public DateTime RegisteredAtUtc { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    public DateTime? RejectedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public DateTime? WaitlistedAtUtc { get; set; }
    public int? WaitlistPosition { get; set; }

    public string? RejectionReason { get; set; }

    public string? CancellationReason { get; set; }

    public ParticipantDto? Participant { get; set; }

    public TicketDto? Ticket { get; set; }
}
