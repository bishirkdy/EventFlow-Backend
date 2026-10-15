using EventFlow.SharedKernel.Domain;

namespace EventFlow.Registration.Domain.Entities;

public sealed class RegistrationAnswer : Entity
{
    // EF Core materialization constructor.
    private RegistrationAnswer()
    {
    }

    public RegistrationAnswer(
        Guid registrationId,
        Guid registrationFormFieldId,
        string value)
    {
        RegistrationId = registrationId;
        RegistrationFormFieldId = registrationFormFieldId;
        Value = value;
    }

    public Guid RegistrationId { get; private set; }

    public Guid RegistrationFormFieldId { get; private set; }

    public string Value { get; private set; } = "";

    public Registration Registration { get; private set; } = null!;

    public RegistrationFormField RegistrationFormField { get; private set; } = null!;
}
