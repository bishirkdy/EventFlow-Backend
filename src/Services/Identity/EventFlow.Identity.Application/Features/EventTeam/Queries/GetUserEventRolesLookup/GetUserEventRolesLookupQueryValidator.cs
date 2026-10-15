using FluentValidation;

namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetUserEventRolesLookup;

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
