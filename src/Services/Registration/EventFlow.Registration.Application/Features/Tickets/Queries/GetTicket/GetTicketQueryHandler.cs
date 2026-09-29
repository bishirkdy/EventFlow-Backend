using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.GetTicket;

public sealed class GetTicketQueryHandler(
    ITicketRepository tickets,
    ICurrentUserService user)
    : IRequestHandler<GetTicketQuery, ApiResponse<TicketDto>>
{
    public async Task<ApiResponse<TicketDto>> Handle(
        GetTicketQuery query,
        CancellationToken cancellationToken)
    {
        var ticket = await tickets.GetForRegistrationAsync(
            query.EventId,
            query.RegistrationId,
            user.UserId,
            cancellationToken);

        return ticket is null
            ? ApiResponse<TicketDto>.Fail(["Ticket not found."])
            : ApiResponse<TicketDto>.Success(ticket.ToDto());
    }
}
