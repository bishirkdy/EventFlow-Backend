using FluentValidation;
namespace EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;

public sealed class UpsertRegistrationFormCommandValidator : AbstractValidator<UpsertRegistrationFormCommand>
{
    public UpsertRegistrationFormCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.Capacity).GreaterThan(0).When(x => x.Request.Capacity.HasValue);
        RuleForEach(x => x.Request.Fields).ChildRules(f =>
        {
            f.RuleFor(x => x.FieldKey).NotEmpty().MaximumLength(100); f.RuleFor(x => x.Label).NotEmpty().MaximumLength(250);
        }
    );
    }
}
