
using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.ReorderNavigationItems
{
    public sealed class ReorderNavigationItemsValidator: AbstractValidator<ReorderNavigationItemsCommand>
    {
        public ReorderNavigationItemsValidator()
        {
            RuleFor(x => x.NavigationMenuId)
                .NotEmpty()
                .WithMessage("Navigation menu ID is required.");

            RuleFor(x => x.ItemIds)
                .NotEmpty()
                .WithMessage("Item IDs are required.");

            RuleFor(x => x.ItemIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate navigation item IDs are not allowed.");
        }
    }
}
