

using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenuById
{
    public sealed class GetNavigationMenuByIdValidator: AbstractValidator<GetNavigationMenuByIdQuery>
    {
        public GetNavigationMenuByIdValidator()
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
