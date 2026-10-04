using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateAnalytics;

public sealed class GetCertificateAnalyticsQueryHandler(
    ICertificateRepository certificates,
    IRegistrationRepository registrations,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        GetCertificateAnalyticsQuery,
        ApiResponse<GetCertificateAnalyticsResponse>>
{
    public async Task<ApiResponse<GetCertificateAnalyticsResponse>> Handle(
        GetCertificateAnalyticsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<GetCertificateAnalyticsResponse>.Fail(
                ["You do not have permission."]);
        }

        var certificateRows = await certificates.GetByEventIdAsync(
            query.EventId,
            cancellationToken);

        var stats = await registrations.GetStatisticsAsync(
            query.EventId,
            cancellationToken);

        var now = DateTime.UtcNow;
        var today = now.Date;

        var revoked = certificateRows.Count(x =>
            x.Status == CertificateStatus.Revoked ||
            x.RevokedAtUtc.HasValue);

        var issuedDates = certificateRows
            .Select(x => x.IssuedAtUtc.Date)
            .ToList();

        var issuedByDay = certificateRows
            .Where(x => x.IssuedAtUtc.Date <= today && x.IssuedAtUtc.Date >= today.AddDays(-29))
            .GroupBy(x => x.IssuedAtUtc.Date)
            .ToDictionary(group => group.Key, group => group.Count());

        var trend = Enumerable
            .Range(0, 30)
            .Select(offset =>
            {
                var day = today.AddDays(-(29 - offset));
                issuedByDay.TryGetValue(day, out var count);

                return new RegistrationTrendPointResponse(
                    day.ToString("yyyy-MM-dd"),
                    count,
                    0,
                    0);
            })
            .ToList();

        return ApiResponse<GetCertificateAnalyticsResponse>.Success(
            new GetCertificateAnalyticsResponse
            {
                EventId = query.EventId,

                TotalIssued = certificateRows.Count,
                Active = certificateRows.Count - revoked,
                Revoked = revoked,
                UniqueRecipients = certificateRows
                    .Select(x => x.UserId)
                    .Distinct()
                    .Count(),

                ApprovedRegistrations = stats.Approved,
                IssuanceRatePercent = stats.Approved == 0
                    ? 0
                    : Math.Round(certificateRows.Count * 100d / stats.Approved, 1),

                IssuedToday = issuedDates.Count(x => x == today),
                IssuedLast7Days = issuedDates.Count(
                    x => x <= today && x >= today.AddDays(-6)),

                FirstIssuedAtUtc = certificateRows.Count == 0
                    ? null
                    : certificateRows.Min(x => x.IssuedAtUtc),

                LastIssuedAtUtc = certificateRows.Count == 0
                    ? null
                    : certificateRows.Max(x => x.IssuedAtUtc),

                IssuedByDay = trend
            },
            "Certificate analytics computed.");
    }
}
