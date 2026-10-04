using EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateAnalytics;

public sealed class GetCertificateAnalyticsResponse
{
    public Guid EventId { get; set; }

    public int TotalIssued { get; set; }

    public int Active { get; set; }

    public int Revoked { get; set; }

    public int UniqueRecipients { get; set; }

    public int ApprovedRegistrations { get; set; }

    public double IssuanceRatePercent { get; set; }

    public int IssuedToday { get; set; }

    public int IssuedLast7Days { get; set; }

    public DateTime? FirstIssuedAtUtc { get; set; }

    public DateTime? LastIssuedAtUtc { get; set; }

    public IReadOnlyList<RegistrationTrendPointResponse> IssuedByDay { get; set; } = [];
}
