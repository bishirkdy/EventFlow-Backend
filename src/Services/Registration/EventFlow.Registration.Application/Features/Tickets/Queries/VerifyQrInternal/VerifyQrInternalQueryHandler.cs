using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQrInternal;

public sealed class VerifyQrInternalQueryHandler(ITicketRepository tickets) : IRequestHandler<VerifyQrInternalQuery,TicketDto>
{
 public async Task<TicketDto> Handle(VerifyQrInternalQuery q,CancellationToken ct)
 {
  var ticket=await tickets.GetByQrCodeAsync(q.EventId,q.QrCodeValue,ct);
  if(ticket is null)throw new NotFoundException("QR code is invalid.");
  if(!ticket.IsActive)throw new ConflictException("Ticket is inactive or revoked.");
  return ticket.ToDto();
 }
}
