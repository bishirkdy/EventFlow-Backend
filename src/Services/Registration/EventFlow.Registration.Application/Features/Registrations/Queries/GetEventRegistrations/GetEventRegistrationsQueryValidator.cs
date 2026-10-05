using FluentValidation;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetEventRegistrations;

public sealed class GetEventRegistrationsQueryValidator
    : AbstractValidator<GetEventRegistrationsQuery>
{
    public GetEventRegistrationsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid registration status.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}
