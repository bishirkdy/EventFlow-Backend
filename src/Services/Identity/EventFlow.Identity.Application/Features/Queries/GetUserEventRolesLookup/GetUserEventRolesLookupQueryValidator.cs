using FluentValidation;

namespace EventFlow.Identity.Application.Features.Queries.GetUserEventRolesLookup;

public sealed class GetUserEventRolesLookupQueryValidator
    : AbstractValidator<GetUserEventRolesLookupQuery>
{
    public GetUserEventRolesLookupQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
