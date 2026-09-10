

using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenusByEvent
{
    public sealed class GetNavigationMenusByEventValidator: AbstractValidator<GetNavigationMenusByEventQuery>
    {
        public GetNavigationMenusByEventValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
