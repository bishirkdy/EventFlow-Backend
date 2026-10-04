using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Features.Analytics.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetProgrammeAnalytics;

public sealed class GetProgrammeAnalyticsQueryHandler(
    ISessionRepository sessions,
    ISectionRepository sections,
    IVenueRepository venues,
    ISpeakerRepository speakers,
    ISponsorRepository sponsors)
    : IRequestHandler<GetProgrammeAnalyticsQuery, GetProgrammeAnalyticsResponse>
{
    public async Task<GetProgrammeAnalyticsResponse> Handle(
        GetProgrammeAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var sessionRows = await sessions.GetByEventIdAsync(request.EventId, cancellationToken);
        var sectionRows = await sections.GetByEventIdAsync(request.EventId, cancellationToken);
        var venueRows = await venues.GetByEventIdAsync(request.EventId, cancellationToken);
        var speakerRows = await speakers.GetByEventIdAsync(request.EventId, cancellationToken);
        var sponsorRows = await sponsors.GetByEventIdAsync(request.EventId, cancellationToken);

        var sessionsWithSpeakerIds = new HashSet<Guid>();

        foreach (var speaker in speakerRows)
        {
            var speakerSessions = await speakers.GetSessionsAsync(speaker.Id, cancellationToken);

            foreach (var session in speakerSessions)
            {
                sessionsWithSpeakerIds.Add(session.Id);
            }
        }

        var sessionsWithVenue = sessionRows.Count(x => x.VenueId.HasValue);
        var sessionsWithSpeaker = sessionRows.Count(x => sessionsWithSpeakerIds.Contains(x.Id));
        var publishedStatus = new[] { "Published", "Scheduled" };

        var sessionHours = sessionRows
            .Where(x => x.StartTimeUtc.HasValue && x.EndTimeUtc.HasValue)
            .Sum(x => (x.EndTimeUtc!.Value - x.StartTimeUtc!.Value).TotalHours);

        return new GetProgrammeAnalyticsResponse
        {
            EventId = request.EventId,

            SectionsTotal = sectionRows.Count,
            SectionsActive = sectionRows.Count(x => x.IsActive),

            SessionsTotal = sessionRows.Count,
            SessionsPublished = sessionRows.Count(x => publishedStatus.Contains(x.Status)),
            SessionsDraft = sessionRows.Count(x => x.Status == "Draft"),

            SessionsWithVenue = sessionsWithVenue,
            SessionsWithoutVenue = sessionRows.Count - sessionsWithVenue,

            SessionsWithSpeaker = sessionsWithSpeaker,
            SessionsWithoutSpeaker = sessionRows.Count - sessionsWithSpeaker,

            SpeakerCoveragePercent = sessionRows.Count == 0
                ? 0
                : Math.Round(sessionsWithSpeaker * 100d / sessionRows.Count, 1),

            TotalSessionHours = Math.Round(sessionHours, 1),

            SessionsBySection = sectionRows
                .OrderBy(x => x.DisplayOrder)
                .Select(section => new SectionCountResponse(
                    section.Id,
                    section.Name,
                    sessionRows.Count(x => x.SectionId == section.Id)))
                .ToList(),

            SessionsByType = sessionRows
                .GroupBy(x => x.SessionType)
                .Select(group => new KeyCountResponse(group.Key, group.Count()))
                .OrderByDescending(x => x.Count)
                .ToList(),

            SessionsByDay = sessionRows
                .Where(x => x.StartTimeUtc.HasValue)
                .GroupBy(x => x.StartTimeUtc!.Value.Date)
                .Select(group => new DayCountResponse(
                    group.Key.ToString("yyyy-MM-dd"),
                    group.Count()))
                .OrderBy(x => x.Date)
                .ToList(),

            SpeakersTotal = speakerRows.Count,
            SpeakersActive = speakerRows.Count(x => x.IsActive),
            AvgSessionsPerSpeaker = speakerRows.Count == 0
                ? 0
                : Math.Round(sessionsWithSpeakerIds.Count / (double)speakerRows.Count, 1),

            SponsorsTotal = sponsorRows.Count,
            SponsorsActive = sponsorRows.Count(x => x.IsActive),
            SponsorsByLevel = sponsorRows
                .GroupBy(x => x.SponsorLevel)
                .Select(group => new KeyCountResponse(group.Key, group.Count()))
                .OrderByDescending(x => x.Count)
                .ToList(),

            VenuesTotal = venueRows.Count,
            VenuesActive = venueRows.Count(x => x.IsActive),
            TotalVenueCapacity = venueRows.Where(x => x.IsActive).Sum(x => x.Capacity),

            VenueLoad = venueRows
                .Select(venue => new VenueLoadResponse(
                    venue.Id,
                    venue.Name,
                    venue.Capacity,
                    sessionRows.Count(x => x.VenueId == venue.Id)))
                .OrderByDescending(x => x.SessionCount)
                .ToList()
        };
    }
}
