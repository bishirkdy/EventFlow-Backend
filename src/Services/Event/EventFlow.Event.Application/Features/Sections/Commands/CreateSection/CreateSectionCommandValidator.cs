using FluentValidation;


namespace EventFlow.Event.Application.Features.Sections.Commands.CreateSection
{
    public sealed class CreateSectionCommandValidator: AbstractValidator<CreateSectionCommand>
    {
        public CreateSectionCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Section name is required.")
                .MaximumLength(200)
                .WithMessage("Section name cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Section description cannot exceed 2000 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Display order cannot be negative.");
        }
    }
}
