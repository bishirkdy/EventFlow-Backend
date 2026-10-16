using EventFlow.Identity.Domain.Enums;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitations;

public sealed record GetEventInvitationsQuery(Guid EventId, EventInvitationType? Type)
    : IRequest<IReadOnlyList<GetEventInvitationsResponse>>;
