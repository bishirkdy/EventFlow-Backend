using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using MediatR;
using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;
using RegistrationStatusEnum = EventFlow.Registration.Domain.Enums.RegistrationStatus;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;

public sealed class GetRegistrationAnalyticsQueryHandler(
    IRegistrationRepository registrations,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        GetRegistrationAnalyticsQuery,
        ApiResponse<GetRegistrationAnalyticsResponse>>
{
    public async Task<ApiResponse<GetRegistrationAnalyticsResponse>> Handle(
        GetRegistrationAnalyticsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<GetRegistrationAnalyticsResponse>.Fail(
                ["You do not have permission."]);
        }

        var stats = await registrations.GetStatisticsAsync(
            query.EventId,
            cancellationToken);

        var rows = await registrations.GetWithParticipantsAsync(
            query.EventId,
            cancellationToken);

        var now = DateTime.UtcNow;
        var today = now.Date;

        var approvalDurations = rows
            .Where(x => x.Status == RegistrationStatusEnum.Approved && x.ApprovedAtUtc.HasValue)
            .Select(x => (x.ApprovedAtUtc!.Value - x.RegisteredAtUtc).TotalHours)
            .Where(x => x >= 0)
            .ToList();

        var rejectionDurations = rows
            .Where(x => x.Status == RegistrationStatusEnum.Rejected && x.RejectedAtUtc.HasValue)
            .Select(x => (x.RejectedAtUtc!.Value - x.RegisteredAtUtc).TotalHours)
            .Where(x => x >= 0)
            .ToList();

        var byDay = rows
            .GroupBy(x => x.RegisteredAtUtc.Date)
            .ToDictionary(
                group => group.Key,
                group => new
                {
                    Total = group.Count(),
                    Approved = group.Count(x => x.Status == RegistrationStatusEnum.Approved),
                    Pending = group.Count(x => x.Status == RegistrationStatusEnum.Pending)
                });

        var windowStart = today.AddDays(-(query.Days - 1));

        var trend = Enumerable
            .Range(0, query.Days)
            .Select(offset =>
            {
                var day = windowStart.AddDays(offset);
                byDay.TryGetValue(day, out var counts);

                return new RegistrationTrendPointResponse(
                    day.ToString("yyyy-MM-dd"),
                    counts?.Total ?? 0,
                    counts?.Approved ?? 0,
                    counts?.Pending ?? 0);
            })
            .ToList();

        var peak = byDay.Count == 0
            ? null
            : byDay
                .OrderByDescending(x => x.Value.Total)
                .ThenBy(x => x.Key)
                .Select(x => new RegistrationTrendPointResponse(
                    x.Key.ToString("yyyy-MM-dd"),
                    x.Value.Total,
                    x.Value.Approved,
                    x.Value.Pending))
                .First();

        var topRejectionReasons = rows
            .Where(x =>
                x.Status == RegistrationStatusEnum.Rejected &&
                !string.IsNullOrWhiteSpace(x.RejectionReason))
            .GroupBy(x => x.RejectionReason!.Trim())
            .Select(group => new RejectionReasonResponse(group.Key, group.Count()))
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Reason)
            .Take(5)
            .ToList();

        var trendDates = rows.Select(x => x.RegisteredAtUtc.Date).ToList();

        return ApiResponse<GetRegistrationAnalyticsResponse>.Success(
            new GetRegistrationAnalyticsResponse
            {
                EventId = query.EventId,

                Total = stats.Total,
                Pending = stats.Pending,
                Approved = stats.Approved,
                Rejected = stats.Rejected,
                Cancelled = stats.Cancelled,
                Waitlisted = stats.Waitlisted,
                Participants = stats.Participants,
                ActiveTickets = stats.ActiveTickets,

                ApprovalRatePercent = stats.Total == 0
                    ? 0
                    : Math.Round(stats.Approved * 100d / stats.Total, 1),

                RejectionRatePercent = stats.Total == 0
                    ? 0
                    : Math.Round(stats.Rejected * 100d / stats.Total, 1),

                WaitlistRatePercent = stats.Total == 0
                    ? 0
                    : Math.Round(stats.Waitlisted * 100d / stats.Total, 1),

                AvgApprovalHours = approvalDurations.Count == 0
                    ? 0
                    : Math.Round(approvalDurations.Average(), 1),

                AvgRejectionHours = rejectionDurations.Count == 0
                    ? 0
                    : Math.Round(rejectionDurations.Average(), 1),

                RegisteredToday = trendDates.Count(x => x == today),

                RegisteredLast7Days = trendDates.Count(
                    x => x <= today && x >= today.AddDays(-6)),

                FirstRegistrationAtUtc = trendDates.Count == 0
                    ? null
                    : rows.Min(x => x.RegisteredAtUtc),

                LastRegistrationAtUtc = trendDates.Count == 0
                    ? null
                    : rows.Max(x => x.RegisteredAtUtc),

                PeakDay = peak,
                Trend = trend,
                TopRejectionReasons = topRejectionReasons
            },
            "Registration analytics computed.");
    }
}
