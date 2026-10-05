using FluentValidation;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationStats;

public sealed class GetRegistrationStatsQueryValidator
    : AbstractValidator<GetRegistrationStatsQuery>
{
    public GetRegistrationStatsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
