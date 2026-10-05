using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Commands.ClaimEventOwner;

public sealed class ClaimEventOwnerCommandValidator : AbstractValidator<ClaimEventOwnerCommand>
{
    public ClaimEventOwnerCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
