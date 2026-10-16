using FluentValidation;

namespace EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitation;

public sealed class GetEventInvitationQueryValidator
    : AbstractValidator<GetEventInvitationQuery>
{
    public GetEventInvitationQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Invitation token is required.");
    }
}
