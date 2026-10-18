using EventFlow.SharedKernel.Validation;
using FluentValidation;
namespace EventFlow.Registration.Application.Features.Registrations.Commands.CreateRegistration;

public sealed class CreateRegistrationCommandValidator : AbstractValidator<CreateRegistrationCommand>
{
    public CreateRegistrationCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.Email).NotEmpty().MaximumLength(320).Must(ValidationRules.IsValidEmail).WithMessage("Email address is invalid.");
        RuleFor(x => x.Request.Phone).MaximumLength(30).Must(ValidationRules.IsValidOptionalPhone).WithMessage("Phone number is invalid.");
    }
}
