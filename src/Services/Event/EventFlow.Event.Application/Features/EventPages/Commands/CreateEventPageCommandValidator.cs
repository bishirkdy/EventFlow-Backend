
using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands
{
    public sealed class CreateEventPageCommandValidator: AbstractValidator<CreateEventPageCommand>
    {
        public CreateEventPageCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Page name is required.")
                .MaximumLength(200)
                .WithMessage("Page name cannot exceed 200 characters.");

            RuleFor(x => x.Slug)
                .NotEmpty()
                .WithMessage("Page slug is required.")
                .MaximumLength(200)
                .WithMessage("Page slug cannot exceed 200 characters.");

            RuleFor(x => x.PageType)
                .NotEmpty()
                .WithMessage("Page type is required.")
                .MaximumLength(100)
                .WithMessage("Page type cannot exceed 100 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Display order cannot be negative.");
        }
    }
}
