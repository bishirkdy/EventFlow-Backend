using FluentValidation;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetRegistrationFeature;

public sealed class GetRegistrationFeatureQueryValidator
    : AbstractValidator<GetRegistrationFeatureQuery>
{
    public GetRegistrationFeatureQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
