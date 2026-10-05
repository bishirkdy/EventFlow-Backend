using FluentValidation;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.DisableEventFeature;

public sealed class DisableEventFeatureCommandValidator
    : AbstractValidator<DisableEventFeatureCommand>
{
    public DisableEventFeatureCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.FeatureId)
            .NotEmpty()
            .WithMessage("Feature ID is required.");
    }
}
