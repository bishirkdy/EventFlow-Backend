namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetTeamAnalytics;

public sealed record GetTeamAnalyticsResponse(
    int TotalMembers,
    int Owners,
    int Organizers,
    int Others,
    int AssignedLast7Days,
    DateTime? LastAssignedAtUtc,
    IReadOnlyList<TeamRoleCountResponse> RoleBreakdown);
