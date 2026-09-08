

using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent
{
    public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            // Event name is required.
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Event name is required.")
                .MaximumLength(200)
                .WithMessage("Event name cannot exceed 200 characters.");

            // Description is optional but has a maximum length.
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description cannot exceed 2000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            // Event type is required.
            RuleFor(x => x.EventType)
                .NotEmpty()
                .WithMessage("Event type is required.")
                .MaximumLength(100)
                .WithMessage("Event type cannot exceed 100 characters.");

            // Subtype is optional.
            RuleFor(x => x.SubType)
                .MaximumLength(100)
                .WithMessage("Event subtype cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubType));

            // Start date must be provided.
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required.");

            // End date must be provided and after the start date.
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be greater than start date.");

            // Time zone is required.
            RuleFor(x => x.TimeZone)
                .NotEmpty()
                .WithMessage("Event time zone is required.")
                .MaximumLength(100)
                .WithMessage("Event time zone cannot exceed 100 characters.");
        }
    }
}
