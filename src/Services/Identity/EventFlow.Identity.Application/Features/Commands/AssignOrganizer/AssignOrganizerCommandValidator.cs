using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.AssignOrganizer;

public sealed class AssignOrganizerCommandValidator : AbstractValidator<AssignOrganizerCommand>
{
    public AssignOrganizerCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(320)
            .WithMessage("Email cannot exceed 320 characters.");
    }
}
