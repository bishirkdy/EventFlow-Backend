namespace EventFlow.Registration.Application.Features.Tickets.Queries.GetTicket;

public sealed record GetTicketQuery(Guid EventId, Guid RegistrationId) : IRequest<TicketDto>;
