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
    IEventFeatureRepository eventFeatures,
    IMapper mapper,
    IUserDirectoryClient userDirectoryClient)
    : IRequestHandler<GetEventOverviewAnalyticsQuery, GetEventOverviewAnalyticsResponse?>
{
    public async Task<GetEventOverviewAnalyticsResponse?> Handle(
        GetEventOverviewAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var eventEntity = await events.GetByIdWithImagesAsync(
            request.EventId,
            cancellationToken);

        if (eventEntity is null)
        {
            return null;
        }

        var sectionRows = await sections.GetByEventIdAsync(request.EventId, cancellationToken);
        var sessionRows = await sessions.GetByEventIdAsync(request.EventId, cancellationToken);
        var venueRows = await venues.GetByEventIdAsync(request.EventId, cancellationToken);
        var speakerRows = await speakers.GetByEventIdAsync(request.EventId, cancellationToken);
        var sponsorRows = await sponsors.GetByEventIdAsync(request.EventId, cancellationToken);
        var pageRows = await pages.GetByEventIdAsync(request.EventId, cancellationToken);
        var pageSectionRows = await pageSections.GetByEventIdAsync(
            request.EventId,
            includeUnpublished: true,
            cancellationToken);
        var navRows = await navigationItems.GetByEventIdAsync(request.EventId, cancellationToken);
        var photoRows = await photos.GetByEventIdAsync(request.EventId, cancellationToken);
        var featureRows = await eventFeatures.GetByEventIdAsync(request.EventId, cancellationToken);

        var eventHeader = mapper.Map<GetEventByIdResponse>(eventEntity);

        eventHeader.CreatedByName = await userDirectoryClient.GetDisplayNameAsync(
            eventEntity.CreatedBy,
            cancellationToken);

        var now = DateTime.UtcNow;

        return new GetEventOverviewAnalyticsResponse
        {
            EventId = eventEntity.Id,
            Event = eventHeader,

            Sections = sectionRows.Count,
            Sessions = sessionRows.Count,
            Venues = venueRows.Count,
            Speakers = speakerRows.Count,
            Sponsors = sponsorRows.Count,
            Pages = pageRows.Count,
            PageSections = pageSectionRows.Count,
            NavigationItems = navRows.Count,
            Photos = photoRows.Count,
            PhotosVisible = photoRows.Count(x => x.IsVisible),
            FeaturesEnabled = featureRows.Count(x => x.IsEnabled),

            DaysUntilStart = (eventEntity.StartDate.Date - now.Date).Days,
            DaysUntilEnd = (eventEntity.EndDate.Date - now.Date).Days,
            ComputedAtUtc = now
        };
    }
}
