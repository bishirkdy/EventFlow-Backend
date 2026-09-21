

using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands.UpdateEventPage
{
    public sealed class UpdateEventPageCommandValidator: AbstractValidator<UpdateEventPageCommand>
    {
        public UpdateEventPageCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Page ID is required.");

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
                .WithMessage(
                    "Slug can contain only lowercase letters, numbers, and hyphens.");

            RuleFor(x => x.PageType)
                .NotEmpty()
                .WithMessage("Page type is required.")
                .MaximumLength(50)
                .WithMessage("Page type cannot exceed 50 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Display order cannot be negative.");
        }
    }
}
