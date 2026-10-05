namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQrInternal;

public sealed record VerifyQrInternalQuery(Guid EventId,string QrCodeValue) : IRequest<TicketDto>;
