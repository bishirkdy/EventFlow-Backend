

using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem
{
    public sealed class UpdateNavigationItemValidator: AbstractValidator<UpdateNavigationItemCommand>
    {
        public UpdateNavigationItemValidator()
        {
            RuleFor(x => x.NavigationMenuId)
                .NotEmpty()
                .WithMessage("Navigation menu ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Navigation item ID is required.");

            RuleFor(x => x.Label)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Navigation item label is required.");

            RuleFor(x => x.Url)
                .MaximumLength(1000);

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Display order must be zero or greater.");
        }
    }
}
