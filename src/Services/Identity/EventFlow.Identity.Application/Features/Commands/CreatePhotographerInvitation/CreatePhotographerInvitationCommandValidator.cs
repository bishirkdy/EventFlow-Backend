using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.CreatePhotographerInvitation;

public sealed class CreatePhotographerInvitationCommandValidator
    : AbstractValidator<CreatePhotographerInvitationCommand>
{
    public CreatePhotographerInvitationCommandValidator()
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
