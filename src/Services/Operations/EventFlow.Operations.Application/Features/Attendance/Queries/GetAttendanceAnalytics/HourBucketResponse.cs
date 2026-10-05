namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceAnalytics;

public sealed record HourBucketResponse(int Hour, int Count);
