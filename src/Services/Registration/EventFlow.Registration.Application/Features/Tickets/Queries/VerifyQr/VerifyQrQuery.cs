namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQr;

public sealed record VerifyQrQuery(Guid EventId, string QrCodeValue) : IRequest<TicketDto>;
