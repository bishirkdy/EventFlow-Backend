using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.GetTicket;

public sealed record GetTicketQuery(Guid EventId, Guid RegistrationId) : IRequest<ApiResponse<TicketDto>>;
