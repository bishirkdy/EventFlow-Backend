namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;

public sealed class GetRegistrationAnalyticsResponse
{
    public Guid EventId { get; set; }

    public int Total { get; set; }

    public int Pending { get; set; }

    public int Approved { get; set; }

    public int Rejected { get; set; }

    public int Cancelled { get; set; }

    public int Waitlisted { get; set; }

    public int Participants { get; set; }

    public int ActiveTickets { get; set; }

    public double ApprovalRatePercent { get; set; }

    public double RejectionRatePercent { get; set; }

    public double WaitlistRatePercent { get; set; }

    public double AvgApprovalHours { get; set; }

    public double AvgRejectionHours { get; set; }

    public int RegisteredToday { get; set; }

    public int RegisteredLast7Days { get; set; }

    public DateTime? FirstRegistrationAtUtc { get; set; }

    public DateTime? LastRegistrationAtUtc { get; set; }

    public RegistrationTrendPointResponse? PeakDay { get; set; }

    public IReadOnlyList<RegistrationTrendPointResponse> Trend { get; set; } = [];

    public IReadOnlyList<RejectionReasonResponse> TopRejectionReasons { get; set; } = [];
}
