namespace EventFlow.Event.Application.Features.Admin.Queries.GetAdminEventStats;

public sealed class GetAdminEventStatsResponse
{
    public int TotalEvents { get; init; }
    public int PublishedEvents { get; init; }
    public int DraftEvents { get; init; }
    public int CancelledEvents { get; init; }
}
