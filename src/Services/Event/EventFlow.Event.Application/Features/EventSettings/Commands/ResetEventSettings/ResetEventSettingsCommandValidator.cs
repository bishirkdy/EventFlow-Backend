using FluentValidation;


namespace EventFlow.Event.Application.Features.EventSettings.Commands.ResetEventSettings
{
    // Command Validator
    internal sealed class ResetEventSettingsCommandValidator: AbstractValidator<ResetEventSettingsCommand>
    {
        public ResetEventSettingsCommandValidator()
        {
            // Validate event ID
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
