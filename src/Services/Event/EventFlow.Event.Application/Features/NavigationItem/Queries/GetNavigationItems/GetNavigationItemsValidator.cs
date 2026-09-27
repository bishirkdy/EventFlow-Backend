using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems
{
    public sealed class GetNavigationItemsValidator : AbstractValidator<GetNavigationItemsQuery>
    {
        public GetNavigationItemsValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}