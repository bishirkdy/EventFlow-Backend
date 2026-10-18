using EventFlow.SharedKernel.Validation;
using FluentValidation;
namespace EventFlow.Registration.Application.Features.Registrations.Commands.UpdateRegistration;

public sealed class UpdateRegistrationCommandValidator : AbstractValidator<UpdateRegistrationCommand>
{
    public UpdateRegistrationCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.RegistrationId).NotEmpty();
        RuleFor(x => x.Request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.Email).NotEmpty().MaximumLength(320).Must(ValidationRules.IsValidEmail).WithMessage("Email address is invalid.");
        RuleFor(x => x.Request.Phone).MaximumLength(30).Must(ValidationRules.IsValidOptionalPhone).WithMessage("Phone number is invalid.");
    }
}
