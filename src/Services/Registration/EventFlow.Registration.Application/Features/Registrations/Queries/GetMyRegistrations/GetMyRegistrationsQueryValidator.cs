
namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetMyRegistrations;

public sealed class GetMyRegistrationsQueryValidator
    : AbstractValidator<GetMyRegistrationsQuery>
{
    public GetMyRegistrationsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
