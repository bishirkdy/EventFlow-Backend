namespace EventFlow.Event.Api.Responses.Admin
{
    public sealed class AdminEventStatsResponse
    {
        public int TotalEvents { get; init; }
        public int PublishedEvents { get; init; }
        public int DraftEvents { get; init; }
        public int CancelledEvents { get; init; }
    }
}
