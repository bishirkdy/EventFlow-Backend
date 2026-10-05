namespace EventFlow.Registration.Application.Contracts.RegistrationForms;

public sealed class UpsertRegistrationFormRequest
{
    public string Name { get; set; } = "Event Registration";
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public CapacityMode CapacityMode { get; set; } = CapacityMode.Unlimited;
    public int? Capacity { get; set; }
    public bool EnableWaitlist { get; set; }
    public DateTime? OpensAtUtc { get; set; }
    public DateTime? ClosesAtUtc { get; set; }
    public List<RegistrationFormFieldRequest> Fields { get; set; } = [];
}
