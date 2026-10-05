using FluentValidation;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.EnableEventFeature;

public sealed class EnableEventFeatureCommandValidator
    : AbstractValidator<EnableEventFeatureCommand>
{
    public EnableEventFeatureCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.FeatureId)
            .NotEmpty()
            .WithMessage("Feature ID is required.");
    }
}
