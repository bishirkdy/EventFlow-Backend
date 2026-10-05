using FluentValidation;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.GetTicket;

public sealed class GetTicketQueryValidator : AbstractValidator<GetTicketQuery>
{
    public GetTicketQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required.");
    }
}
