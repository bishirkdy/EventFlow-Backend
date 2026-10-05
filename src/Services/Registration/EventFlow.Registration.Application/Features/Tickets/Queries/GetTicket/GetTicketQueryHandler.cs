using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.GetTicket;

public sealed class GetTicketQueryHandler(ITicketRepository tickets,EventFlow.Security.Authentication.ICurrentUserService user) : IRequestHandler<GetTicketQuery, TicketDto>
{
    public async Task<TicketDto> Handle(GetTicketQuery query, CancellationToken cancellationToken)
    {
        var ticket = await tickets.GetForRegistrationAsync(
            query.EventId,
            query.RegistrationId,
            user.UserId,
            cancellationToken);

        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found.");
        }

        return ticket.ToDto();
    }
}
