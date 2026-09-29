using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Domain.Entities;

public sealed class Participant
{
    public Guid Id { get; set; }

    public Guid RegistrationId { get; set; }

    public Guid EventId { get; set; }

    public Guid UserId { get; set; }

    public string ParticipantNumber { get; set; } = "";

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";

    public string? Phone { get; set; }

    public string? Organization { get; set; }

    public string? Designation { get; set; }

    public ParticipantStatus Status { get; set; } = ParticipantStatus.Active;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public Registration Registration { get; set; } = null!;

    public Ticket? Ticket { get; set; }
}