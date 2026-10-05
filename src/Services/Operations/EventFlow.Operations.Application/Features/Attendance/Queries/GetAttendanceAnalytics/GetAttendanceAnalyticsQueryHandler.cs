using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceAnalytics;

public sealed class GetAttendanceAnalyticsQueryHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    EventFlow.Security.Authentication.ICurrentUserService user)
    : IRequestHandler<
        GetAttendanceAnalyticsQuery,
        GetAttendanceAnalyticsResponse>
{
    public async Task<GetAttendanceAnalyticsResponse> Handle(
        GetAttendanceAnalyticsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await authorization.HasPermissionAsync(
                user.UserId,
                query.EventId,
                "event.view",
                cancellationToken))
        {
            throw new ForbiddenException(
                "You do not have permission to view attendance analytics.");
        }

        var records = await db.AttendanceRecords
            .Where(x => x.EventId == query.EventId)
            .ToListAsync(cancellationToken);

        if (records.Count == 0)
        {
            var empty = new GetAttendanceAnalyticsResponse
            {
                EventId = query.EventId,
                CheckInsByHour = Enumerable
                    .Range(0, 24)
                    .Select(hour => new HourBucketResponse(hour, 0))
                    .ToList(),
                CheckInsByDay = Enumerable
                    .Range(0, query.Days)
                    .Select(offset =>
                        new AttendanceDayCountResponse(
                            DateTime.UtcNow.Date.AddDays(-(query.Days - 1) + offset)
                                .ToString("yyyy-MM-dd"),
                            0))
                    .ToList()
            };

            return empty;
        }

        var now = DateTime.UtcNow;
        var today = now.Date;

        var checkedOutRecords = records
            .Where(x => x.CheckedOutAtUtc.HasValue)
            .ToList();

        var dwellMinutes = checkedOutRecords
            .Select(x => (x.CheckedOutAtUtc!.Value - x.CheckedInAtUtc).TotalMinutes)
            .Where(x => x >= 0)
            .ToList();

        var hourCounts = new int[24];
        foreach (var record in records)
        {
            hourCounts[record.CheckedInAtUtc.Hour]++;
        }

        var peakHourCount = hourCounts.Max();

        var windowStart = today.AddDays(-(query.Days - 1));
        var byDay = records
            .Where(x => x.CheckedInAtUtc.Date >= windowStart && x.CheckedInAtUtc.Date <= today)
            .GroupBy(x => x.CheckedInAtUtc.Date)
            .ToDictionary(group => group.Key, group => group.Count());

        var checkInsByDay = Enumerable
            .Range(0, query.Days)
            .Select(offset =>
            {
                var day = windowStart.AddDays(offset);
                byDay.TryGetValue(day, out var count);

                return new AttendanceDayCountResponse(
                    day.ToString("yyyy-MM-dd"),
                    count);
            })
            .ToList();

        var distinctParticipants = records
            .Select(x => x.RegistrationId)
            .Distinct()
            .Count();

        return new GetAttendanceAnalyticsResponse
        {
            EventId = query.EventId,

            TotalCheckIns = records.Count,
            DistinctParticipants = distinctParticipants,
            CheckedOut = checkedOutRecords.Count,
            CurrentlyInside = records.Count - checkedOutRecords.Count,

            CheckOutRatePercent = records.Count == 0
                ? 0
                : Math.Round(checkedOutRecords.Count * 100d / records.Count, 1),

            SessionsCovered = records
                .Count(x => x.SessionId.HasValue),
            SectionsCovered = records
                .Count(x => x.SectionId.HasValue),
            ActiveStaff = records
                .Select(x => x.StaffUserId)
                .Distinct()
                .Count(),

            QrCheckIns = records.Count(x => x.Method == AttendanceMethod.Qr),
            ManualCheckIns = records.Count(x => x.Method == AttendanceMethod.Manual),

            PeakHour = peakHourCount > 0
                ? Array.IndexOf(hourCounts, peakHourCount)
                : null,
            PeakHourCheckIns = peakHourCount,

            CheckInsByHour = hourCounts
                .Select((count, hour) => new HourBucketResponse(hour, count))
                .ToList(),

            CheckInsByDay = checkInsByDay,

            AvgDwellMinutes = dwellMinutes.Count == 0
                ? 0
                : Math.Round(dwellMinutes.Average(), 1),

            FirstCheckInAtUtc = records.Min(x => x.CheckedInAtUtc),
            LastCheckInAtUtc = records.Max(x => x.CheckedInAtUtc)
        };
    }
}
