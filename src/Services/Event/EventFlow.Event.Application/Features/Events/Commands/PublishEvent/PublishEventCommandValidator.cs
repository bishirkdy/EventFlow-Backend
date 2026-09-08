using FluentValidation;


namespace EventFlow.Event.Application.Features.Events.Commands.PublishEvent
{
    // Command Validator
    internal sealed class PublishEventCommandValidator: AbstractValidator<PublishEventCommand>
    {
        public PublishEventCommandValidator()
        {
            // Validate event ID
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
