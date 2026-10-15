namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetTeamAnalytics;

public sealed record TeamRoleCountResponse(Guid RoleId, string RoleName, int Count);
