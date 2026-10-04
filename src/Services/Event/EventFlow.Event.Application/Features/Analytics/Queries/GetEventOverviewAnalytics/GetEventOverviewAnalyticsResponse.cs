using EventFlow.Event.Application.Features.Events.Queries.GetEventById;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetEventOverviewAnalytics;

public sealed class GetEventOverviewAnalyticsResponse
{
    public Guid EventId { get; init; }

    public GetEventByIdResponse? Event { get; init; }

    public int Sections { get; init; }

    public int Sessions { get; init; }

    public int Venues { get; init; }

    public int Speakers { get; init; }

    public int Sponsors { get; init; }

    public int Pages { get; init; }

    public int PageSections { get; init; }

    public int NavigationItems { get; init; }

    public int Photos { get; init; }

    public int PhotosVisible { get; init; }

    public int FeaturesEnabled { get; init; }

    public int DaysUntilStart { get; init; }

    public int DaysUntilEnd { get; init; }

    public DateTime ComputedAtUtc { get; init; }
}
