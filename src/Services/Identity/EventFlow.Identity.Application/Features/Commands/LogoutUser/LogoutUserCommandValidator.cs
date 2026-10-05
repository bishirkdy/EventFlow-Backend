using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.LogoutUser;

public sealed class LogoutUserCommandValidator : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .MaximumLength(4096)
            .When(x => x.RefreshToken is not null)
            .WithMessage("Refresh token cannot exceed 4096 characters.");
    }
}
