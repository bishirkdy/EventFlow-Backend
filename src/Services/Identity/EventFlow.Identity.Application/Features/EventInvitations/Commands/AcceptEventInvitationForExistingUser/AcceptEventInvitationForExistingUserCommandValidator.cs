using FluentValidation;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitationForExistingUser;

public sealed class AcceptEventInvitationForExistingUserCommandValidator
    : AbstractValidator<AcceptEventInvitationForExistingUserCommand>
{
    public AcceptEventInvitationForExistingUserCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Invitation token is required.");
    }
}
