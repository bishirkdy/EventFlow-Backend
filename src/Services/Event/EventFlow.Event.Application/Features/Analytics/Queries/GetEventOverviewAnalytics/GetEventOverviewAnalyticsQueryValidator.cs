using FluentValidation;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetEventOverviewAnalytics;

internal sealed class GetEventOverviewAnalyticsQueryValidator
    : AbstractValidator<GetEventOverviewAnalyticsQuery>
{
    public GetEventOverviewAnalyticsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
