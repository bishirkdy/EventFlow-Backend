using FluentValidation;

namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetTeamAnalytics;

public sealed class GetTeamAnalyticsQueryValidator
    : AbstractValidator<GetTeamAnalyticsQuery>
{
    public GetTeamAnalyticsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
