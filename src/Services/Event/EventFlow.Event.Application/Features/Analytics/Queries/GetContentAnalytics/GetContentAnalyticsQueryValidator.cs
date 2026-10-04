using FluentValidation;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetContentAnalytics;

internal sealed class GetContentAnalyticsQueryValidator
    : AbstractValidator<GetContentAnalyticsQuery>
{
    public GetContentAnalyticsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
