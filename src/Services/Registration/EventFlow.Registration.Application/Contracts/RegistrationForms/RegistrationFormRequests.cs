
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
    public List<RegistrationFormFieldRequest> Fields { get; set; } = new();
}

public sealed class RegistrationFormFieldRequest
{
    public Guid? Id { get; set; }
    public string FieldKey { get; set; } = "";
    public string Label { get; set; } = "";
    public RegistrationFieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? OptionsJson { get; set; }
    public string? ValidationJson { get; set; }
}