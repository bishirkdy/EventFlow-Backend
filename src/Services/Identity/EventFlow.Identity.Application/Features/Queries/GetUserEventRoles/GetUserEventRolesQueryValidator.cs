using FluentValidation;

namespace EventFlow.Identity.Application.Features.Queries.GetUserEventRoles;

public sealed class GetUserEventRolesQueryValidator : AbstractValidator<GetUserEventRolesQuery>
{
    public GetUserEventRolesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
