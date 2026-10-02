namespace EventFlow.Operations.Application.Contracts;

public sealed record AttendanceDashboardDto(
    int TotalParticipants,
    int CheckedIn,
    int CheckedOut,
    int CurrentlyInside,
    double AttendancePercentage);
