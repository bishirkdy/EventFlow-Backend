using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandValidator
    : AbstractValidator<RefreshAccessTokenCommand>
{
    public RefreshAccessTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .MaximumLength(4096)
            .When(x => x.RefreshToken is not null)
            .WithMessage("Refresh token cannot exceed 4096 characters.");
    }
}
