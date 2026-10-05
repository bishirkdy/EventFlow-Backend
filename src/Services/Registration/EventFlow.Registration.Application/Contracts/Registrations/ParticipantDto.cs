using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Contracts.Registrations;

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
