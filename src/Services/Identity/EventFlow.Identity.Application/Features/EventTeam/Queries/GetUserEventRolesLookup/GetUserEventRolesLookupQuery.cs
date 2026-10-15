using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetUserEventRolesLookup;

public sealed record GetUserEventRolesLookupQuery(Guid UserId)
    : IRequest<IReadOnlyList<UserEventRoleLookupResponse>>;
