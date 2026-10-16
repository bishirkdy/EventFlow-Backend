using FluentValidation;

namespace EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents;

public sealed class GetAdminEventsQueryValidator : AbstractValidator<GetAdminEventsQuery>
{
    public GetAdminEventsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}
