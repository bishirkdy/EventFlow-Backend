using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands.CreateEventPage
{
    public sealed class CreateEventPageCommandValidator
        : AbstractValidator<CreateEventPageCommand>
    {
        public CreateEventPageCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Page name is required.")
                .MaximumLength(150)
                .WithMessage("Page name cannot exceed 150 characters.");

            RuleFor(x => x.Slug)
                .NotEmpty()
                .WithMessage("Page slug is required.")
                .MaximumLength(150)
                .WithMessage("Page slug cannot exceed 150 characters.")
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug can contain only lowercase letters, numbers, and hyphens.");

            RuleFor(x => x.PageType)
                .NotEmpty()
                .WithMessage("Page type is required.")
                .MaximumLength(50)
                .WithMessage("Page type cannot exceed 50 characters.");
        }
    }
}