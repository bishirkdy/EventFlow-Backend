namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceAnalytics;

public sealed class GetAttendanceAnalyticsResponse
{
    public Guid EventId { get; init; }

    public int TotalCheckIns { get; init; }

    public int DistinctParticipants { get; init; }

    public int CheckedOut { get; init; }

    public int CurrentlyInside { get; init; }

    public double CheckOutRatePercent { get; init; }

    public int SessionsCovered { get; init; }

    public int SectionsCovered { get; init; }

    public int ActiveStaff { get; init; }

    public int QrCheckIns { get; init; }

    public int ManualCheckIns { get; init; }

    public int? PeakHour { get; init; }

    public int PeakHourCheckIns { get; init; }

    public IReadOnlyList<HourBucketResponse> CheckInsByHour { get; init; } = [];

    public IReadOnlyList<AttendanceDayCountResponse> CheckInsByDay { get; init; } = [];

    public double AvgDwellMinutes { get; init; }

    public DateTime? FirstCheckInAtUtc { get; init; }

    public DateTime? LastCheckInAtUtc { get; init; }
}
