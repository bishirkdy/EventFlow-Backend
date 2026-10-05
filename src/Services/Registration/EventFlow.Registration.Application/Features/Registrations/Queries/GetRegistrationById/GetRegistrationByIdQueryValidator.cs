using FluentValidation;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationById;

public sealed class GetRegistrationByIdQueryValidator
    : AbstractValidator<GetRegistrationByIdQuery>
{
    public GetRegistrationByIdQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required.");
    }
}
