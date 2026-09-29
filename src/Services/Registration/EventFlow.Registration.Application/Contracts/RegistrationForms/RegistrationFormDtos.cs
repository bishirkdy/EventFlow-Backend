namespace EventFlow.Registration.Application.Contracts.RegistrationForms;

public sealed class RegistrationFormDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public CapacityMode CapacityMode { get; set; }
    public int? Capacity { get; set; }
    public int ApprovedCount { get; set; }
    public int WaitlistCount { get; set; }
    public bool EnableWaitlist { get; set; }
    public DateTime? OpensAtUtc { get; set; }
    public DateTime? ClosesAtUtc { get; set; }
    public List<RegistrationFormFieldDto> Fields { get; set; } = new();
}

public sealed class RegistrationFormFieldDto
{
    public Guid Id { get; set; }
    public string FieldKey { get; set; } = "";
    public string Label { get; set; } = "";
    public RegistrationFieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? OptionsJson { get; set; }
    public string? ValidationJson { get; set; }
}