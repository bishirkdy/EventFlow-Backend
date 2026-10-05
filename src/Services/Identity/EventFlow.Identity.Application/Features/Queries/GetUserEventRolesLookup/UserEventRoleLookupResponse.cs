namespace EventFlow.Identity.Application.Features.Queries.GetUserEventRolesLookup;

public sealed record UserEventRoleLookupResponse(
    Guid EventId,
    IReadOnlyList<string> RoleNames);
