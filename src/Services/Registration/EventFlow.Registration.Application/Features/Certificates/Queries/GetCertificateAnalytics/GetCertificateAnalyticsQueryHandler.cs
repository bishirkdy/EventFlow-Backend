using EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;
using EventFlow.SharedKernel.Exceptions;
using EventFlow.Security.Authentication;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateAnalytics;

public sealed class GetCertificateAnalyticsQueryHandler(
    ICertificateRepository certificates,
    IRegistrationRepository registrations,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<GetCertificateAnalyticsQuery, GetCertificateAnalyticsResponse>
{
    public async Task<GetCertificateAnalyticsResponse> Handle(
        GetCertificateAnalyticsQuery query, CancellationToken cancellationToken)
    {
        // Check whether the current user has permission to manage registrations.
        if (!await access.CanManageRegistrationAsync(query.EventId, user.UserId, cancellationToken))
        {
            // Stop the request if the user does not have permission.
            throw new ForbiddenException("You do not have permission.");
        }

        // Get all certificates belonging to the event.
        var certificateRows = await certificates.GetByEventIdAsync(query.EventId, cancellationToken);

        // Get registration statistics for the event.
        var stats = await registrations.GetStatisticsAsync(query.EventId, cancellationToken);

        // Get the current UTC date and time.
        var now = DateTime.UtcNow;
        var today = now.Date;

        // Count certificates that have been revoked.
        var revoked = certificateRows.Count(x =>
            x.Status == CertificateStatus.Revoked || x.RevokedAtUtc.HasValue);

        // Get only the dates when certificates were issued.
        var issuedDates = certificateRows.Select(x => x.IssuedAtUtc.Date).ToList();

        // Group certificates issued during the last 30 days by date.
        var issuedByDay = certificateRows
            .Where(x => x.IssuedAtUtc.Date <= today && x.IssuedAtUtc.Date >= today.AddDays(-29))
            .GroupBy(x => x.IssuedAtUtc.Date)
            .ToDictionary(group => group.Key, group => group.Count());

        // Create a 30-day trend from oldest day to today.
        var trend = Enumerable
            .Range(0, 30)
            .Select(offset =>
            {
                // Calculate the date for this position in the 30-day range.
                var day = today.AddDays(-(29 - offset));

                // Try to get the number of certificates issued on this day.
                // If there are none, count remains 0.
                issuedByDay.TryGetValue(day, out var count);

                // Create one trend point for this day.
                return new RegistrationTrendPointResponse(
                    day.ToString("yyyy-MM-dd"), count, 0, 0);
            })
            .ToList();

        // Build and return the certificate analytics response.
        return new GetCertificateAnalyticsResponse
        {
            // ID of the event being analysed.
            EventId = query.EventId,

            // Total number of certificates.
            TotalIssued = certificateRows.Count,

            // Number of active certificates.
            Active = certificateRows.Count - revoked,

            // Number of revoked certificates.
            Revoked = revoked,

            // Count unique users who received certificates.
            UniqueRecipients = certificateRows
                .Select(x => x.UserId)
                .Distinct()
                .Count(),

            // Number of approved registrations.
            ApprovedRegistrations = stats.Approved,

            // Calculate the percentage of approved registrations that received certificates.
            IssuanceRatePercent = stats.Approved == 0
                ? 0
                : Math.Round(certificateRows.Count * 100d / stats.Approved, 1),

            // Count certificates issued today.
            IssuedToday = issuedDates.Count(x => x == today),

            // Count certificates issued during the last 7 days.
            IssuedLast7Days = issuedDates.Count(
                x => x <= today && x >= today.AddDays(-6)),

            // Get the date and time of the first certificate issued.
            // Return null if there are no certificates.
            FirstIssuedAtUtc = certificateRows.Count == 0
                ? null
                : certificateRows.Min(x => x.IssuedAtUtc),

            // Get the date and time of the most recently issued certificate.
            // Return null if there are no certificates.
            LastIssuedAtUtc = certificateRows.Count == 0
                ? null
                : certificateRows.Max(x => x.IssuedAtUtc),

            // Include the 30-day certificate issuance trend.
            IssuedByDay = trend
        };
    }
}