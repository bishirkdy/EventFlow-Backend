using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetEventTeam;

public sealed record GetEventTeamQuery(Guid EventId)
    : IRequest<IReadOnlyList<EventTeamMemberResponse>>;
