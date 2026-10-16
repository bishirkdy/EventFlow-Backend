using FluentValidation;

namespace EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitations;

public sealed class GetEventInvitationsQueryValidator
    : AbstractValidator<GetEventInvitationsQuery>
{
    public GetEventInvitationsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
