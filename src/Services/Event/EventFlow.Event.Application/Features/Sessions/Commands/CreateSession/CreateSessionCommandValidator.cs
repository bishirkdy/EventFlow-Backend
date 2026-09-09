using FluentValidation;


namespace EventFlow.Event.Application.Features.Sessions.Commands.CreateSession
{
    public sealed class CreateSessionCommandValidator
        : AbstractValidator<CreateSessionCommand>
    {
        public CreateSessionCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.SectionId)
                .NotEmpty()
                .WithMessage("Section ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Session title is required.")
                .MaximumLength(200)
                .WithMessage("Session title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Session description cannot exceed 2000 characters.");

            RuleFor(x => x.SessionType)
                .NotEmpty()
                .WithMessage("Session type is required.")
                .MaximumLength(100)
                .WithMessage("Session type cannot exceed 100 characters.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .When(x => x.Capacity.HasValue)
                .WithMessage("Session capacity must be greater than 0.");
        }
    }
}
