using FluentValidation;

namespace EventFlow.Identity.Application.Features.Queries.GetEventTeam;

public sealed class GetEventTeamQueryValidator : AbstractValidator<GetEventTeamQuery>
{
    public GetEventTeamQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
