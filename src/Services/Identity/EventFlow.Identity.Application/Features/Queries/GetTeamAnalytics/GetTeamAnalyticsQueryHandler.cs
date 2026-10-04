using EventFlow.Identity.Application.Abstractions.Repositories;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetTeamAnalytics;

public sealed class GetTeamAnalyticsQueryHandler(
    IUserEventRoleRepository userEventRoleRepository)
    : IRequestHandler<GetTeamAnalyticsQuery, GetTeamAnalyticsResponse>
{
    public async Task<GetTeamAnalyticsResponse> Handle(
        GetTeamAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var assignments = await userEventRoleRepository.GetByEventAsync(
            request.EventId,
            cancellationToken);

        var roleBreakdown = assignments
            .GroupBy(x => new { x.RoleId, RoleName = x.Role.Name })
            .Select(group => new TeamRoleCountResponse(
                group.Key.RoleId,
                group.Key.RoleName,
                group.Select(x => x.UserId).Distinct().Count()))
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.RoleName)
            .ToList();

        var members = assignments
            .GroupBy(x => x.UserId)
            .Select(group => new
            {
                group.Key,
                RoleNames = group.Select(x => x.Role.Name).ToHashSet(StringComparer.OrdinalIgnoreCase)
            })
            .ToList();

        var now = DateTime.UtcNow;

        return new GetTeamAnalyticsResponse(
            TotalMembers: members.Count,
            Owners: members.Count(x => x.RoleNames.Contains("Owner")),
            Organizers: members.Count(x => x.RoleNames.Contains("Organizer")),
            Others: members.Count(x =>
                !x.RoleNames.Contains("Owner") &&
                !x.RoleNames.Contains("Organizer")),
            AssignedLast7Days: assignments.Count(x => x.AssignedAt >= now.AddDays(-7)),
            LastAssignedAtUtc: assignments.Count == 0
                ? null
                : assignments.Max(x => x.AssignedAt),
            RoleBreakdown: roleBreakdown);
    }
}
