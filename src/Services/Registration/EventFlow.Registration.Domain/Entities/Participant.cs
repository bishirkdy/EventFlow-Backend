using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Registration.Domain.Entities;

public sealed class Participant : Entity
{
    // EF Core materialization constructor.
    private Participant()
    {
    }

    public Participant(
        Guid registrationId,
        Guid eventId,
        Guid userId,
        string participantNumber,
        string firstName,
        string lastName,
        string email,
        string? phone,
        string? organization,
        string? designation)
    {
        RegistrationId = registrationId;
        EventId = eventId;
        UserId = userId;
        ParticipantNumber = participantNumber;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Organization = organization;
        Designation = designation;
        Status = ParticipantStatus.Active;
    }

    public Guid RegistrationId { get; private set; }

    public Guid EventId { get; private set; }

    public Guid UserId { get; private set; }

    public string ParticipantNumber { get; private set; } = "";

    public string FirstName { get; private set; } = "";

    public string LastName { get; private set; } = "";

    public string Email { get; private set; } = "";

    public string? Phone { get; private set; }

    public string? Organization { get; private set; }

    public string? Designation { get; private set; }

    public ParticipantStatus Status { get; private set; } = ParticipantStatus.Active;

    public Registration Registration { get; private set; } = null!;

    public Ticket? Ticket { get; private set; }

    // Updates the editable participant details.
    public void Update(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string? organization,
        string? designation)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Organization = organization;
        Designation = designation;
        SetUpdatedAt();
    }

    // Marks the participant as cancelled.
    public void Cancel()
    {
        Status = ParticipantStatus.Cancelled;
        SetUpdatedAt();
    }
}
