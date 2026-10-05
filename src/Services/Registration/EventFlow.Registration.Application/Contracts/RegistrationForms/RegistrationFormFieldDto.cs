namespace EventFlow.Registration.Application.Contracts.RegistrationForms;

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
