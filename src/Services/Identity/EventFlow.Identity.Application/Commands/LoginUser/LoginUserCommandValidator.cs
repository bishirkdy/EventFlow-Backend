using EventFlow.Identity.Application.Commands.Login;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Identity.Application.Commands.LoginUser
{
    public sealed class LoginUserCommandValidator: AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Please enter a valid email address.")
                .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
