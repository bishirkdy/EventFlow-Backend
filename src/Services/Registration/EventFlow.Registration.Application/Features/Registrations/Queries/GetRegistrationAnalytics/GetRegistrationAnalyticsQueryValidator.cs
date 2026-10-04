using FluentValidation;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;

internal sealed class GetRegistrationAnalyticsQueryValidator
    : AbstractValidator<GetRegistrationAnalyticsQuery>
{
    public GetRegistrationAnalyticsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Days)
            .InclusiveBetween(7, 90)
            .WithMessage("Days must be between 7 and 90.");
    }
}
