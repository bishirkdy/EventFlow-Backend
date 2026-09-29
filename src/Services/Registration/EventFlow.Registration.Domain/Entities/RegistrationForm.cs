using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Domain.Entities;

public sealed class RegistrationForm
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public string Name { get; set; } = "Event Registration";

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public CapacityMode CapacityMode { get; set; } = CapacityMode.Unlimited;

    public int? Capacity { get; set; }

    public bool EnableWaitlist { get; set; }

    public DateTime? OpensAtUtc { get; set; }

    public DateTime? ClosesAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<RegistrationFormField> Fields { get; set; } = new List<RegistrationFormField>();
}