using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem
{
    public sealed class UpdateNavigationItemValidator : AbstractValidator<UpdateNavigationItemCommand>
    {
        public UpdateNavigationItemValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Navigation item ID is required.");

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