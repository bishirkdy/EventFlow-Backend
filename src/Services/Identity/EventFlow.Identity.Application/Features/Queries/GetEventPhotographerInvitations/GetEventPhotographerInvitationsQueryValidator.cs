using FluentValidation;

namespace EventFlow.Identity.Application.Features.Queries.GetEventPhotographerInvitations;

public sealed class GetEventPhotographerInvitationsQueryValidator
    : AbstractValidator<GetEventPhotographerInvitationsQuery>
{
    public GetEventPhotographerInvitationsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
