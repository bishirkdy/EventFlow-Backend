using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Domain.Entities;

public sealed class RegistrationFormField
{
    public Guid Id { get; set; }

    public Guid RegistrationFormId { get; set; }

    public string FieldKey { get; set; } = "";

    public string Label { get; set; } = "";

    public RegistrationFieldType FieldType { get; set; }

    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }

    public string? OptionsJson { get; set; }

    public string? ValidationJson { get; set; }

    public RegistrationForm RegistrationForm { get; set; } = null!;


    public ICollection<RegistrationAnswer> Answers { get; set; } = new List<RegistrationAnswer>();
}