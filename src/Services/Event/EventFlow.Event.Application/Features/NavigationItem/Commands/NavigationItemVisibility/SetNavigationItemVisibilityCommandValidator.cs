
using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.NavigationItemVisibility
{
    public sealed class SetNavigationItemVisibilityCommandValidator: AbstractValidator<SetNavigationItemVisibilityCommand>
    {
        public SetNavigationItemVisibilityCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Navigation menu ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Navigation item ID is required.");
        }
    }
}
