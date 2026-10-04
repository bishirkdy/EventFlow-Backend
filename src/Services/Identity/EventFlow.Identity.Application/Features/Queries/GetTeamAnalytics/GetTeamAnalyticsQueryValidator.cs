using FluentValidation;

namespace EventFlow.Identity.Application.Features.Queries.GetTeamAnalytics;

internal sealed class GetTeamAnalyticsQueryValidator
    : AbstractValidator<GetTeamAnalyticsQuery>
{
    public GetTeamAnalyticsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
