namespace EventFlow.Identity.Application.Abstractions.ReadModels;

// Aggregate user counts used by the platform admin dashboard.
public sealed record UserCountStats(
    int TotalUsers,
    int ActiveUsers,
    int PlatformAdmins);
