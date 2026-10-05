using FluentValidation;

namespace EventFlow.Identity.Application.Features.Commands.RevokePhotographerInvitation;

public sealed class RevokePhotographerInvitationCommandValidator
    : AbstractValidator<RevokePhotographerInvitationCommand>
{
    public RevokePhotographerInvitationCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.InvitationId)
            .NotEmpty()
            .WithMessage("Invitation ID is required.");
    }
}
