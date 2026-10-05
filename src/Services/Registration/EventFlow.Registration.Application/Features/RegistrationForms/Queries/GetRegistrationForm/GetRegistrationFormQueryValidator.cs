using FluentValidation;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;

public sealed class GetRegistrationFormQueryValidator
    : AbstractValidator<GetRegistrationFormQuery>
{
    public GetRegistrationFormQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
