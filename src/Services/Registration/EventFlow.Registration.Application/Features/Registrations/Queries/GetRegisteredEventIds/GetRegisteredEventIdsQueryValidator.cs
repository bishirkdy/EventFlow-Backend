using FluentValidation;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegisteredEventIds;

public sealed class GetRegisteredEventIdsQueryValidator
    : AbstractValidator<GetRegisteredEventIdsQuery>
{
    public GetRegisteredEventIdsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
