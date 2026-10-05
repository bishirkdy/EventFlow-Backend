using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems
{
    public sealed class GetNavigationItemsQueryValidator : AbstractValidator<GetNavigationItemsQuery>
    {
        public GetNavigationItemsQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}