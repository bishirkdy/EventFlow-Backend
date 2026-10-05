using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.RemoveOrganizer;

public sealed class RemoveOrganizerCommandValidator : AbstractValidator<RemoveOrganizerCommand>
{
    public RemoveOrganizerCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
