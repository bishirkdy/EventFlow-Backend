

using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationMenu.Commands.DeleteNavigationMenu
{
    public sealed class DeleteNavigationMenuValidator: AbstractValidator<DeleteNavigationMenuCommand>
    {
        public DeleteNavigationMenuValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Navigation menu ID is required.");
        }
    }
}
