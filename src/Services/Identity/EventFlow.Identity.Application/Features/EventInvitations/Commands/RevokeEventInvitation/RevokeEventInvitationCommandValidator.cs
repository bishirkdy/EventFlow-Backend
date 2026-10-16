using FluentValidation;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.RevokeEventInvitation;

public sealed class RevokeEventInvitationCommandValidator
    : AbstractValidator<RevokeEventInvitationCommand>
{
    public RevokeEventInvitationCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.InvitationId)
            .NotEmpty()
            .WithMessage("Invitation ID is required.");
    }
}
