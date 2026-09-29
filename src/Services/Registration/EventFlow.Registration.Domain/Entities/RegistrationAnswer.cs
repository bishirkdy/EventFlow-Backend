namespace EventFlow.Registration.Domain.Entities;

public sealed class RegistrationAnswer
{
    public Guid Id { get; set; }

    public Guid RegistrationId { get; set; }

    public Guid RegistrationFormFieldId { get; set; }

    public string Value { get; set; } = "";

    public Registration Registration { get; set; } = null!;

    public RegistrationFormField RegistrationFormField { get; set; } = null!;
}