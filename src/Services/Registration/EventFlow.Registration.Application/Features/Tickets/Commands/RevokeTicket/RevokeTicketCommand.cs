using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.Tickets.Commands.RevokeTicket;

public sealed record RevokeTicketCommand(Guid EventId, Guid TicketId) : IRequest<ApiResponse<TicketDto>>;
