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
    public async Task<GetProgrammeAnalyticsResponse> Handle(GetProgrammeAnalyticsQuery request, CancellationToken cancellationToken)
    {
        // Get all sessions for the event.
        var sessionRows = await sessions.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all sections for the event.
        var sectionRows = await sections.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all venues for the event.
        var venueRows = await venues.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all speakers for the event.
        var speakerRows = await speakers.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all sponsors for the event.
        var sponsorRows = await sponsors.GetByEventIdAsync(request.EventId, cancellationToken);

        // Store session IDs that have at least one speaker.
        var sessionsWithSpeakerIds = new HashSet<Guid>();

        // Check each speaker in the event.
        foreach (var speaker in speakerRows)
        {
            // Get all sessions assigned to this speaker.
            var speakerSessions = await speakers.GetSessionsAsync(speaker.Id, cancellationToken);

            // Add the session IDs to the HashSet.
            foreach (var session in speakerSessions)
            {
                sessionsWithSpeakerIds.Add(session.Id);
            }
        }

        // Count sessions that have a venue assigned.
        var sessionsWithVenue = sessionRows.Count(x => x.VenueId.HasValue);

        // Count sessions that have at least one speaker.
        var sessionsWithSpeaker = sessionRows.Count(x => sessionsWithSpeakerIds.Contains(x.Id));

        // Define the statuses considered published or scheduled.
        var publishedStatus = new[] { "Published", "Scheduled" };

        // Calculate the total duration of all sessions in hours.
        var sessionHours = sessionRows
            .Where(x => x.StartTimeUtc.HasValue && x.EndTimeUtc.HasValue)
            .Sum(x => (x.EndTimeUtc!.Value - x.StartTimeUtc!.Value).TotalHours);

        // Build the programme analytics response.
        return new GetProgrammeAnalyticsResponse
        {
            // ID of the event being analysed.
            EventId = request.EventId,

            // Section statistics.
            SectionsTotal = sectionRows.Count,
            SectionsActive = sectionRows.Count(x => x.IsActive),

            // Session statistics.
            SessionsTotal = sessionRows.Count,
            SessionsPublished = sessionRows.Count(x => publishedStatus.Contains(x.Status)),
            SessionsDraft = sessionRows.Count(x => x.Status == "Draft"),

            // Sessions with and without a venue.
            SessionsWithVenue = sessionsWithVenue,
            SessionsWithoutVenue = sessionRows.Count - sessionsWithVenue,

            // Sessions with and without a speaker.
            SessionsWithSpeaker = sessionsWithSpeaker,
            SessionsWithoutSpeaker = sessionRows.Count - sessionsWithSpeaker,

            // Calculate the percentage of sessions that have speakers.
            SpeakerCoveragePercent = sessionRows.Count == 0 ? 0 : Math.Round(sessionsWithSpeaker * 100d / sessionRows.Count, 1),

            // Total duration of all sessions in hours.
            TotalSessionHours = Math.Round(sessionHours, 1),

            // Count sessions grouped by section.
            SessionsBySection = sectionRows
                .OrderBy(x => x.DisplayOrder)
                .Select(section => new SectionCountResponse(
                    section.Id,
                    section.Name,
                    sessionRows.Count(x => x.SectionId == section.Id)))
                .ToList(),

            // Count sessions grouped by session type.
            SessionsByType = sessionRows
                .GroupBy(x => x.SessionType)
                .Select(group => new KeyCountResponse(group.Key, group.Count()))
                .OrderByDescending(x => x.Count)
                .ToList(),

            // Count sessions grouped by day.
            SessionsByDay = sessionRows
                .Where(x => x.StartTimeUtc.HasValue)
                .GroupBy(x => x.StartTimeUtc!.Value.Date)
                .Select(group => new DayCountResponse(
                    group.Key.ToString("yyyy-MM-dd"),
                    group.Count()))
                .OrderBy(x => x.Date)
                .ToList(),

            // Speaker statistics.
            SpeakersTotal = speakerRows.Count,
            SpeakersActive = speakerRows.Count(x => x.IsActive),
            AvgSessionsPerSpeaker = speakerRows.Count == 0 ? 0 : Math.Round(sessionsWithSpeakerIds.Count / (double)speakerRows.Count, 1),

            // Sponsor statistics.
            SponsorsTotal = sponsorRows.Count,
            SponsorsActive = sponsorRows.Count(x => x.IsActive),

            // Count sponsors grouped by sponsor level.
            SponsorsByLevel = sponsorRows
                .GroupBy(x => x.SponsorLevel)
                .Select(group => new KeyCountResponse(group.Key, group.Count()))
                .OrderByDescending(x => x.Count)
                .ToList(),

            // Venue statistics.
            VenuesTotal = venueRows.Count,
            VenuesActive = venueRows.Count(x => x.IsActive),

            // Calculate the total capacity of active venues.
            TotalVenueCapacity = venueRows.Where(x => x.IsActive).Sum(x => x.Capacity),

            // Calculate how many sessions are assigned to each venue.
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
