using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Registrations;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQrInternal;

public sealed record VerifyQrInternalQuery(Guid EventId,string QrCodeValue) : IRequest<ApiResponse<TicketDto>>;
