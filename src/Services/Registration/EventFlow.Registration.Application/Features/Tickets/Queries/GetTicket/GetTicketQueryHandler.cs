using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.GetTicket;

public sealed class GetTicketQueryHandler(ITicketRepository tickets,EventFlow.Security.Authentication.ICurrentUserService user) : IRequestHandler<GetTicketQuery, ApiResponse<TicketDto>>
{
    public async Task<ApiResponse<TicketDto>> Handle(GetTicketQuery query, CancellationToken cancellationToken)
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
