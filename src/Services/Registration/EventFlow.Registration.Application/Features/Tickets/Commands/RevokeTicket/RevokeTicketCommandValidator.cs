using FluentValidation;

namespace EventFlow.Registration.Application.Features.Tickets.Commands.RevokeTicket;

public sealed class RevokeTicketCommandValidator : AbstractValidator<RevokeTicketCommand>
{
    public RevokeTicketCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");
    }
}
