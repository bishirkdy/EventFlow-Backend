using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.DeleteNavigationItem
{
    public sealed class DeleteNavigationItemCommandValidator : AbstractValidator<DeleteNavigationItemCommand>
    {
        public DeleteNavigationItemCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Navigation item ID is required.");
        }
    }
}