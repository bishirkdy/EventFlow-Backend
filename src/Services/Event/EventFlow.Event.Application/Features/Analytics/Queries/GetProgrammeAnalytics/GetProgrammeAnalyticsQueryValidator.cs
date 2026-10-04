using FluentValidation;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetProgrammeAnalytics;

internal sealed class GetProgrammeAnalyticsQueryValidator
    : AbstractValidator<GetProgrammeAnalyticsQuery>
{
    public GetProgrammeAnalyticsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
