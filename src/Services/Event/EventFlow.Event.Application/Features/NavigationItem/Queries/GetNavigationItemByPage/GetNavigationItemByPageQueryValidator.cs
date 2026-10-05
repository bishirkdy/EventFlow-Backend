using FluentValidation;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItemByPage;

public sealed class GetNavigationItemByPageQueryValidator
    : AbstractValidator<GetNavigationItemByPageQuery>
{
    public GetNavigationItemByPageQueryValidator()
    {
        RuleFor(x => x.PageId)
            .NotEmpty()
            .WithMessage("Page ID is required.");
    }
}
