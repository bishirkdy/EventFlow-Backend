using FluentValidation;


namespace EventFlow.Event.Application.Features.Events.Commands.CancelEvent
{
    // Command Validator
    internal sealed class CancelEventCommandValidator
        : AbstractValidator<CancelEventCommand>
    {
        public CancelEventCommandValidator()
        {
            // Validate event ID
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
