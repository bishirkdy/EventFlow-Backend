using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQrInternal;

public sealed class VerifyQrInternalQueryHandler(ITicketRepository tickets) : IRequestHandler<VerifyQrInternalQuery,ApiResponse<TicketDto>>
{
 public async Task<ApiResponse<TicketDto>> Handle(VerifyQrInternalQuery q,CancellationToken ct)
 {
  var ticket=await tickets.GetByQrCodeAsync(q.EventId,q.QrCodeValue,ct);
  if(ticket is null)return ApiResponse<TicketDto>.Fail(["QR code is invalid."]);
  if(!ticket.IsActive)return ApiResponse<TicketDto>.Fail(["Ticket is inactive or revoked."]);
  return ApiResponse<TicketDto>.Success(ticket.ToDto(),"Ticket is valid.");
 }
}
