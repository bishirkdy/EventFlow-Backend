using EventFlow.SharedKernel.Validation;
using FluentValidation;
namespace EventFlow.Identity.Application.Features.Authentication.Commands.RegisterUser
{
    //Validation for registration
    public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .Matches("^[a-zA-Z0-9._-]+$")
                .WithMessage(
                    "Username can contain only letters, numbers, '.', '_' and '-'.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(255)
                .Must(ValidationRules.IsValidEmail)
                .WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100)
                .Matches("[A-Z]")
                .WithMessage(
                    "Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                .WithMessage(
                    "Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                .WithMessage(
                    "Password must contain at least one number.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
