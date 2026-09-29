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

public sealed class ParticipantDto
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public Guid RegistrationId { get; set; }

    public Guid UserId { get; set; }

    public string ParticipantNumber { get; set; } = "";

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";

    public string? Phone { get; set; }

    public string? Organization { get; set; }

    public string? Designation { get; set; }

    public ParticipantStatus Status { get; set; }
}

public sealed class TicketDto
{
    public Guid Id { get; set; }

    public Guid RegistrationId { get; set; }

    public Guid ParticipantId { get; set; }

    public string TicketNumber { get; set; } = "";

    public string QrCodeValue { get; set; } = "";

    public DateTime IssuedAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public bool IsActive { get; set; }
}