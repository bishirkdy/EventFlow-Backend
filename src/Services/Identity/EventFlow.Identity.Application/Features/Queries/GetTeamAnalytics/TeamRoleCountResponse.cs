namespace EventFlow.Identity.Application.Features.Queries.GetTeamAnalytics;

public sealed record TeamRoleCountResponse(Guid RoleId, string RoleName, int Count);
