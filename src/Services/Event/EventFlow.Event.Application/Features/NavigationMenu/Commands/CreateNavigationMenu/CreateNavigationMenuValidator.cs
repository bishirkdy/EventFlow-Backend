

using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationMenu.Commands.CreateNavigationMenu
{
    public sealed class CreateNavigationMenuValidator: AbstractValidator<CreateNavigationMenuCommand>
    {
        public CreateNavigationMenuValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Navigation menu name is required.");

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Navigation menu location is required.");
        }
    }
}
