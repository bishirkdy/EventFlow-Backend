using FluentValidation;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetRegistrationAccess;

public sealed class GetRegistrationAccessQueryValidator
    : AbstractValidator<GetRegistrationAccessQuery>
{
    public GetRegistrationAccessQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
