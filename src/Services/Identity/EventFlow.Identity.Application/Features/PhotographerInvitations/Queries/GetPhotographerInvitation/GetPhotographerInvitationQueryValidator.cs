using FluentValidation;

namespace EventFlow.Identity.Application.Features.PhotographerInvitations.Queries.GetPhotographerInvitation;

public sealed class GetPhotographerInvitationQueryValidator
    : AbstractValidator<GetPhotographerInvitationQuery>
{
    public GetPhotographerInvitationQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Invitation token is required.");
    }
}
