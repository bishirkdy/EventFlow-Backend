using FluentValidation;


namespace EventFlow.Event.Application.Features.EventFeature.Commands.ResetEventFeatures
{
    public sealed class ResetEventFeaturesCommandValidator: AbstractValidator<ResetEventFeaturesCommand>
    {
        public ResetEventFeaturesCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
