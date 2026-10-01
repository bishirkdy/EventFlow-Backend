using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQr;

public sealed class VerifyQrQueryHandler(ITicketRepository tickets,EventFlow.Security.Authentication.ICurrentUserService user,IEventRegistrationAccessService access)
    : IRequestHandler<VerifyQrQuery, ApiResponse<TicketDto>>
{
    public async Task<ApiResponse<TicketDto>> Handle(VerifyQrQuery query, CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(query.EventId,user.UserId, cancellationToken))
        {
            return ApiResponse<TicketDto>.Fail(["You do not have permission."]);
        }

        var ticket = await tickets.GetByQrCodeAsync(query.EventId,query.QrCodeValue,cancellationToken);

        if (ticket is null)
        {
            return ApiResponse<TicketDto>.Fail(["QR code is invalid."]);
        }

        if (!ticket.IsActive)
        {
            return ApiResponse<TicketDto>.Fail(["Ticket is inactive or revoked."]);
        }

        return ApiResponse<TicketDto>.Success(ticket.ToDto(), "Ticket is valid.");
    }
}
