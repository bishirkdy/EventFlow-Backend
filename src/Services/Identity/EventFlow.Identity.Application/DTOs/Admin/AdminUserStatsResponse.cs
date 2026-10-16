namespace EventFlow.Identity.Application.DTOs.Admin;

public sealed record AdminUserStatsResponse(
    int TotalUsers,
    int ActiveUsers,
    int PlatformAdmins);
