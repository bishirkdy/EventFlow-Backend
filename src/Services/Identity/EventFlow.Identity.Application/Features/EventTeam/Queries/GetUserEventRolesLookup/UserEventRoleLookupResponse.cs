namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetUserEventRolesLookup;

public sealed record UserEventRoleLookupResponse(
    Guid EventId,
    IReadOnlyList<string> RoleNames);
