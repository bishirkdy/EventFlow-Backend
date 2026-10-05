using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem
{
    public sealed class CreateNavigationItemCommandValidator : AbstractValidator<CreateNavigationItemCommand>
    {
        public CreateNavigationItemCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Label)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Navigation item label is required.");

            RuleFor(x => x.PageId)
                .NotEmpty()
                .WithMessage("Navigation page is required.");
        }
    }
}