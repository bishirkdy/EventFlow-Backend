using EventFlow.Event.Application.Features.Analytics.Common;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetProgrammeAnalytics;

public sealed class GetProgrammeAnalyticsResponse
{
    public Guid EventId { get; init; }

    public int SectionsTotal { get; init; }

    public int SectionsActive { get; init; }

    public int SessionsTotal { get; init; }

    public int SessionsPublished { get; init; }

    public int SessionsDraft { get; init; }

    public int SessionsWithVenue { get; init; }

    public int SessionsWithoutVenue { get; init; }

    public int SessionsWithSpeaker { get; init; }

    public int SessionsWithoutSpeaker { get; init; }

    public double SpeakerCoveragePercent { get; init; }

    public double TotalSessionHours { get; init; }

    public IReadOnlyList<SectionCountResponse> SessionsBySection { get; init; } = [];

    public IReadOnlyList<KeyCountResponse> SessionsByType { get; init; } = [];

    public IReadOnlyList<DayCountResponse> SessionsByDay { get; init; } = [];

    public int SpeakersTotal { get; init; }

    public int SpeakersActive { get; init; }

    public double AvgSessionsPerSpeaker { get; init; }

    public int SponsorsTotal { get; init; }

    public int SponsorsActive { get; init; }

    public IReadOnlyList<KeyCountResponse> SponsorsByLevel { get; init; } = [];

    public int VenuesTotal { get; init; }

    public int VenuesActive { get; init; }

    public int TotalVenueCapacity { get; init; }

    public IReadOnlyList<VenueLoadResponse> VenueLoad { get; init; } = [];
}
