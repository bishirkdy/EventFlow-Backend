using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Tickets.Commands.RevokeTicket;

public sealed record RevokeTicketCommand(Guid EventId, Guid TicketId) : IRequest<TicketDto>;
