

using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.DeleteNavigationItem
{
    public sealed class DeleteNavigationItemValidator: AbstractValidator<DeleteNavigationItemCommand>
    {
        public DeleteNavigationItemValidator()
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
