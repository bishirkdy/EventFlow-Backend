using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Features.Analytics.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetContentAnalytics;

public sealed class GetContentAnalyticsQueryHandler(
    IEventPageRepository pages,
    IPageSectionRepository pageSections,
    INavigationItemRepository navigationItems,
    IEventPhotoRepository photos,
    IEventFeatureRepository eventFeatures)
    : IRequestHandler<GetContentAnalyticsQuery, GetContentAnalyticsResponse>
{
    public async Task<GetContentAnalyticsResponse> Handle(
        GetContentAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        // Get all pages belonging to the event.
        var pageRows = await pages.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all page sections, including unpublished sections.
        var sectionRows = await pageSections.GetByEventIdAsync(request.EventId, includeUnpublished: true, cancellationToken);

        // Get all navigation items belonging to the event.
        var navRows = await navigationItems.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all photos belonging to the event.
        var photoRows = await photos.GetByEventIdAsync(request.EventId, cancellationToken);

        // Get all features configured for the event.
        var featureRows = await eventFeatures.GetByEventIdAsync(request.EventId, cancellationToken);

        // Create a dictionary using page ID as the key and page name as the value.
        var pageNames = pageRows.ToDictionary(x => x.Id, x => x.Name);

        // Get only the features that are currently enabled.
        var enabledFeatures = featureRows.Where(x => x.IsEnabled).ToList();

        // Build and return the content analytics response.
        return new GetContentAnalyticsResponse
        {
            // ID of the event being analysed.
            EventId = request.EventId,

            // Page statistics.
            PagesTotal = pageRows.Count,
            PagesPublished = pageRows.Count(x => x.IsPublished),
            PagesDraft = pageRows.Count(x => !x.IsPublished),

            // Page section statistics.
            PageSectionsTotal = sectionRows.Count,
            PageSectionsVisible = sectionRows.Count(x => x.IsVisible),

            // Count page sections for each page.
            SectionsByPage = pageRows
                .OrderBy(x => x.DisplayOrder)
                .Select(page => new PageSectionCountResponse(
                    page.Id,
                    page.Name,
                    sectionRows.Count(x => x.PageId == page.Id)))
                .ToList(),

            // Group page sections by their section type.
            SectionsByType = sectionRows
                .GroupBy(x => x.SectionType)
                .Select(group => new KeyCountResponse(group.Key, group.Count()))
                .OrderByDescending(x => x.Count)
                .ToList(),

            // Navigation item statistics.
            NavigationItemsTotal = navRows.Count,
            NavigationItemsVisible = navRows.Count(x => x.IsVisible),

            // Photo statistics.
            PhotosTotal = photoRows.Count,
            PhotosVisible = photoRows.Count(x => x.IsVisible),

            // Photos that are not visible are considered pending approval.
            PhotosPendingApproval = photoRows.Count(x => !x.IsVisible),

            // Feature statistics.
            FeaturesTotal = featureRows.Count,
            FeaturesEnabled = enabledFeatures.Count,

            // Get the names of all enabled features.
            EnabledFeatureNames = enabledFeatures
                .Select(x => x.Feature?.Name ?? string.Empty)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList()
        };
    }
}