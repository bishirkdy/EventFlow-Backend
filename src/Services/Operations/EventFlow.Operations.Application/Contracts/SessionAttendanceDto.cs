namespace EventFlow.Operations.Application.Contracts;

public sealed record SessionAttendanceDto(
    Guid SessionId,
    int TotalParticipants,
    int CheckedIn,
    int CheckedOut,
    int CurrentlyInside,
    double AttendancePercentage);
