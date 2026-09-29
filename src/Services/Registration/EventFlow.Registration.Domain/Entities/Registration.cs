using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Domain.Entities;

public sealed class Registration
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public Guid UserId { get; set; }

    public string RegistrationNumber { get; set; } = "";

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

    public DateTime RegisteredAtUtc { get; set; }

    public DateTime? ApprovedAtUtc { get; set; }

    public DateTime? RejectedAtUtc { get; set; }

    public DateTime? CancelledAtUtc { get; set; }

    public DateTime? WaitlistedAtUtc { get; set; }

    public int? WaitlistPosition { get; set; }

    public string? RejectionReason { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public Participant? Participant { get; set; }

    public ICollection<RegistrationAnswer> Answers { get; set; } = new List<RegistrationAnswer>();

    public Ticket? Ticket { get; set; }
}