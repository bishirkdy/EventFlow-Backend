using FluentValidation;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures;

public sealed class GetEventFeaturesQueryValidator : AbstractValidator<GetEventFeaturesQuery>
{
    public GetEventFeaturesQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
