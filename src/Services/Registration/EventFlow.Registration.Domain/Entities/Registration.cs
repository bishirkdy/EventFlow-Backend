using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Registration.Domain.Entities;

public sealed class Registration : Entity
{
    // EF Core materialization constructor.
    private Registration()
    {
    }

    public Registration(
        Guid eventId,
        Guid userId,
        string registrationNumber,
        RegistrationStatus status,
        int? waitlistPosition = null)
    {
        EventId = eventId;
        UserId = userId;
        RegistrationNumber = registrationNumber;
        Status = status;
        RegisteredAtUtc = DateTime.UtcNow;

        // Waitlisted registrations remember when and where they joined the queue.
        if (status == RegistrationStatus.Waitlisted)
        {
            WaitlistedAtUtc = RegisteredAtUtc;
            WaitlistPosition = waitlistPosition;
        }
    }

    public Guid EventId { get; private set; }

    public Guid UserId { get; private set; }

    public string RegistrationNumber { get; private set; } = "";

    public RegistrationStatus Status { get; private set; } = RegistrationStatus.Pending;

    public DateTime RegisteredAtUtc { get; private set; }

    public DateTime? ApprovedAtUtc { get; private set; }

    public DateTime? RejectedAtUtc { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public DateTime? WaitlistedAtUtc { get; private set; }

    public int? WaitlistPosition { get; private set; }

    public string? RejectionReason { get; private set; }

    public string? CancellationReason { get; private set; }

    public Participant? Participant { get; private set; }

    public ICollection<RegistrationAnswer> Answers { get; private set; } = new List<RegistrationAnswer>();

    public Ticket? Ticket { get; private set; }

    // Links the submitted participant details to this registration.
    public void SetParticipant(Participant participant)
    {
        Participant = participant;
    }

    // Approves the registration and clears any waitlist state.
    public void Approve()
    {
        Status = RegistrationStatus.Approved;
        ApprovedAtUtc = DateTime.UtcNow;
        WaitlistedAtUtc = null;
        WaitlistPosition = null;
        SetUpdatedAt();
    }

    // Rejects the registration with a reason and revokes any issued ticket.
    public void Reject(string reason)
    {
        Status = RegistrationStatus.Rejected;
        RejectionReason = reason;
        RejectedAtUtc = DateTime.UtcNow;
        WaitlistedAtUtc = null;
        WaitlistPosition = null;
        Ticket?.Revoke();
        SetUpdatedAt();
    }

    // Cancels the registration, the participant and any issued ticket.
    public void Cancel(string? reason)
    {
        Status = RegistrationStatus.Cancelled;
        CancellationReason = reason;
        CancelledAtUtc = DateTime.UtcNow;
        Participant?.Cancel();
        Ticket?.Revoke();
        SetUpdatedAt();
    }

    // Moves the registration onto the waitlist at the given position.
    public void Waitlist(int position)
    {
        Status = RegistrationStatus.Waitlisted;
        WaitlistedAtUtc = DateTime.UtcNow;
        WaitlistPosition = position;
        SetUpdatedAt();
    }

    // Attaches a freshly issued ticket.
    public void AttachTicket(Ticket ticket)
    {
        Ticket = ticket;
    }

    // Updates the participant details submitted with the registration.
    public void UpdateParticipant(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string? organization,
        string? designation)
    {
        Participant?.Update(
            firstName,
            lastName,
            email,
            phone,
            organization,
            designation);

        SetUpdatedAt();
    }

    // Removes the current answers and returns them so the store can delete them.
    public IReadOnlyList<RegistrationAnswer> ClearAnswers()
    {
        var answers = Answers.ToList();
        Answers.Clear();
        return answers;
    }

    // Adds a single form answer to the registration.
    public void AddAnswer(Guid registrationFormFieldId, string value)
    {
        Answers.Add(new RegistrationAnswer(Id, registrationFormFieldId, value));
    }
}
