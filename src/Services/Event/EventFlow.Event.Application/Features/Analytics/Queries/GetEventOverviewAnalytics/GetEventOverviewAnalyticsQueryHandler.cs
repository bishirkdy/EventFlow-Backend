using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using MediatR;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetEventOverviewAnalytics;

public sealed class GetEventOverviewAnalyticsQueryHandler(
    IEventRepository events,
    ISectionRepository sections,
    ISessionRepository sessions,
    IVenueRepository venues,
    ISpeakerRepository speakers,
    ISponsorRepository sponsors,
    IEventPageRepository pages,
    IPageSectionRepository pageSections,
    INavigationItemRepository navigationItems,
    IEventPhotoRepository photos,
    IFeedbackRepository feedback,
    IEventFeatureRepository eventFeatures,
    IMapper mapper,
    IUserDirectoryClient userDirectoryClient)
    : IRequestHandler<GetEventOverviewAnalyticsQuery, GetEventOverviewAnalyticsResponse?>
{
    public async Task<GetEventOverviewAnalyticsResponse?> Handle(GetEventOverviewAnalyticsQuery request, CancellationToken cancellationToken)
    {
        // Get the event details along with its images.
        var eventEntity = await events.GetByIdWithImagesAsync(request.EventId, cancellationToken);

        if (eventEntity is null)
        {
            return null;
        }

        // Get all sections belonging to this event.
        var sectionRows = await sections.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all sessions belonging to this event.
        var sessionRows = await sessions.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all venues belonging to this event.
        var venueRows = await venues.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all speakers belonging to this event.
        var speakerRows = await speakers.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all sponsors belonging to this event.
        var sponsorRows = await sponsors.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all pages belonging to this event.
        var pageRows = await pages.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all page sections, including unpublished ones.
        var pageSectionRows = await pageSections.GetByEventIdAsync(request.EventId, includeUnpublished: true, cancellationToken);

        // Get all navigation items belonging to this event.
        var navRows = await navigationItems.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all photos belonging to this event.
        var photoRows = await photos.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all feedback submitted for this event.
        var feedbackRows = await feedback.GetByEventAsync(request.EventId, cancellationToken);

        // Get all features configured for this event.
        var featureRows = await eventFeatures.GetByEventIdAsync(request.EventId, cancellationToken);

        // Map the event entity to the response model.
        var eventHeader = mapper.Map<GetEventByIdResponse>(eventEntity);

        // Get the display name of the user who created the event.
        eventHeader.CreatedByName = await userDirectoryClient.GetDisplayNameAsync(eventEntity.CreatedBy, cancellationToken);

        // Get the current UTC date and time.
        var now = DateTime.UtcNow;

        return new GetEventOverviewAnalyticsResponse
        {
            EventId = eventEntity.Id,
            Event = eventHeader,

            // Count the different event-related records.
            Sections = sectionRows.Count,
            Sessions = sessionRows.Count,
            Venues = venueRows.Count,
            Speakers = speakerRows.Count,
            Sponsors = sponsorRows.Count,
            Pages = pageRows.Count,
            PageSections = pageSectionRows.Count,
            NavigationItems = navRows.Count,
            Photos = photoRows.Count,

            // Count only photos that are currently visible.
            PhotosVisible = photoRows.Count(x => x.IsVisible),

            // Total number of feedback records.
            FeedbackCount = feedbackRows.Count,

            // Calculate the average feedback rating.
            FeedbackAverageRating = feedbackRows.Count == 0 ? 0d : Math.Round(feedbackRows.Average(x => x.Rating), 2),

            // Count only the features that are enabled.
            FeaturesEnabled = featureRows.Count(x => x.IsEnabled),

            // Calculate how many days remain until the event starts.
            DaysUntilStart = (eventEntity.StartDate.Date - now.Date).Days,

            // Calculate how many days remain until the event ends.
            DaysUntilEnd = (eventEntity.EndDate.Date - now.Date).Days,

            // Store the time when these analytics were calculated.
            ComputedAtUtc = now
        };
    }
}