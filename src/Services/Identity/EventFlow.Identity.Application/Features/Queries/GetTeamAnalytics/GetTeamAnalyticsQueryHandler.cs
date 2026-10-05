using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetTeamAnalytics;

public sealed class GetTeamAnalyticsQueryHandler(
    IUserEventRoleRepository userEventRoleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<GetTeamAnalyticsQuery, GetTeamAnalyticsResponse>
{
    public async Task<GetTeamAnalyticsResponse> Handle(
        GetTeamAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                request.EventId,
                PermissionConstants.Event.View,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to view this event.");
        }

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
