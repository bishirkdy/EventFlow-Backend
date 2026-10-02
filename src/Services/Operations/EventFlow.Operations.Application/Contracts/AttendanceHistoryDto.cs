namespace EventFlow.Operations.Application.Contracts;

public sealed record AttendanceHistoryDto(
    Guid EventId,
    int EventAttendanceCount,
    int SessionAttendanceCount,
    int TotalSessions,
    double AttendancePercentage);
