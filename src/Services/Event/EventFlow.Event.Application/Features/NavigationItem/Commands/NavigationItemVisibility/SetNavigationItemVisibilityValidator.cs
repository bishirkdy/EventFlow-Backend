
using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.NavigationItemVisibility
{
    public sealed class SetNavigationItemVisibilityValidator: AbstractValidator<SetNavigationItemVisibilityCommand>
    {
        public SetNavigationItemVisibilityValidator()
        {
            RuleFor(x => x.NavigationMenuId)
                .NotEmpty()
                .WithMessage("Navigation menu ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Navigation item ID is required.");
        }
    }
}
