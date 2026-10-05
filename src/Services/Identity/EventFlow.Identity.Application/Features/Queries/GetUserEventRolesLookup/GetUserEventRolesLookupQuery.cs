using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetUserEventRolesLookup;

public sealed record GetUserEventRolesLookupQuery(Guid UserId)
    : IRequest<IReadOnlyList<UserEventRoleLookupResponse>>;
