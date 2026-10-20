namespace EventFlow.Event.Application.Features.Events.Queries.GetPublicEventStats;

public sealed class GetPublicEventStatsResponse
{
    public int PublishedEvents { get; init; }

    public int Participants { get; init; }
}
