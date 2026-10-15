using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Registration.Domain.Entities;

public sealed class RegistrationForm : Entity
{
    // EF Core materialization constructor.
    private RegistrationForm()
    {
    }

    public RegistrationForm(Guid eventId)
    {
        EventId = eventId;
    }

    public Guid EventId { get; private set; }

    public string Name { get; private set; } = "Event Registration";

    public string? Description { get; private set; }

    public bool IsActive { get; private set; } = true;

    public CapacityMode CapacityMode { get; private set; } = CapacityMode.Unlimited;

    public int? Capacity { get; private set; }

    public bool EnableWaitlist { get; private set; }

    public DateTime? OpensAtUtc { get; private set; }

    public DateTime? ClosesAtUtc { get; private set; }

    public ICollection<RegistrationFormField> Fields { get; private set; } = new List<RegistrationFormField>();

    // Applies the editable form configuration.
    public void Update(
        string name,
        string? description,
        bool isActive,
        CapacityMode capacityMode,
        int? capacity,
        bool enableWaitlist,
        DateTime? opensAtUtc,
        DateTime? closesAtUtc)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
        CapacityMode = capacityMode;
        Capacity = capacity;
        EnableWaitlist = enableWaitlist;
        OpensAtUtc = opensAtUtc;
        ClosesAtUtc = closesAtUtc;
        SetUpdatedAt();
    }
}
