using FluentValidation;

namespace EventFlow.Identity.Application.Features.Queries.GetUserSummary;

public sealed class GetUserSummaryQueryValidator : AbstractValidator<GetUserSummaryQuery>
{
    public GetUserSummaryQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
