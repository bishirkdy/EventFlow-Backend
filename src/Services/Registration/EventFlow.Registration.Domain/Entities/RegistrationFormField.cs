using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Registration.Domain.Entities;

public sealed class RegistrationFormField : Entity
{
    // EF Core materialization constructor.
    private RegistrationFormField()
    {
    }

    // Creates a new field with a generated identifier.
    public RegistrationFormField(
        Guid registrationFormId,
        string fieldKey,
        string label,
        RegistrationFieldType fieldType,
        bool isRequired,
        int displayOrder,
        string? optionsJson,
        string? validationJson)
        : this(
            Guid.NewGuid(),
            registrationFormId,
            fieldKey,
            label,
            fieldType,
            isRequired,
            displayOrder,
            optionsJson,
            validationJson)
    {
    }

    // Creates a field while keeping an existing identifier.
    public RegistrationFormField(
        Guid id,
        Guid registrationFormId,
        string fieldKey,
        string label,
        RegistrationFieldType fieldType,
        bool isRequired,
        int displayOrder,
        string? optionsJson,
        string? validationJson)
        : base(id)
    {
        RegistrationFormId = registrationFormId;
        FieldKey = fieldKey;
        Label = label;
        FieldType = fieldType;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        OptionsJson = optionsJson;
        ValidationJson = validationJson;
    }

    public Guid RegistrationFormId { get; private set; }

    public string FieldKey { get; private set; } = "";

    public string Label { get; private set; } = "";

    public RegistrationFieldType FieldType { get; private set; }

    public bool IsRequired { get; private set; }

    public int DisplayOrder { get; private set; }

    public string? OptionsJson { get; private set; }

    public string? ValidationJson { get; private set; }

    public RegistrationForm RegistrationForm { get; private set; } = null!;

    public ICollection<RegistrationAnswer> Answers { get; private set; } = new List<RegistrationAnswer>();

    // Updates the editable field configuration.
    public void Update(
        string fieldKey,
        string label,
        RegistrationFieldType fieldType,
        bool isRequired,
        int displayOrder,
        string? optionsJson,
        string? validationJson)
    {
        FieldKey = fieldKey;
        Label = label;
        FieldType = fieldType;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        OptionsJson = optionsJson;
        ValidationJson = validationJson;
        SetUpdatedAt();
    }
}
