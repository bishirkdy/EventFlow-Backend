

using FluentValidation;

namespace EventFlow.Event.Application.Features.EventSettings.Commands.UpdateEventSettings
{
    // Command Validator
    internal sealed class UpdateEventSettingsCommandValidator: AbstractValidator<UpdateEventSettingsCommand>
    {
        public UpdateEventSettingsCommandValidator()
        {
            // Validate event ID
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            // Validate default language
            RuleFor(x => x.DefaultLanguage)
                .NotEmpty()
                .MaximumLength(10)
                .WithMessage("Default language is required.");
        }
    }
}
