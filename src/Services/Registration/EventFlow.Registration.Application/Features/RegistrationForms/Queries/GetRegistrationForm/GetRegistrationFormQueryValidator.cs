using EventFlow.Registration.Domain.Enums;
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

        RuleFor(x => x.ScopeType)
            .IsInEnum()
            .WithMessage("Invalid registration scope.");

        RuleFor(x => x.ScopeId)
            .NotEmpty()
            .WithMessage("A session is required for a session registration form.")
            .When(x => x.ScopeType == RegistrationScope.Session);
    }
}
