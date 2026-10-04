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
        var pageRows = await pages.GetByEventIdAsync(request.EventId, cancellationToken);
        var sectionRows = await pageSections.GetByEventIdAsync(
            request.EventId,
            includeUnpublished: true,
            cancellationToken);
        var navRows = await navigationItems.GetByEventIdAsync(request.EventId, cancellationToken);
        var photoRows = await photos.GetByEventIdAsync(request.EventId, cancellationToken);
        var featureRows = await eventFeatures.GetByEventIdAsync(request.EventId, cancellationToken);

        var pageNames = pageRows.ToDictionary(x => x.Id, x => x.Name);
        var enabledFeatures = featureRows.Where(x => x.IsEnabled).ToList();

        return new GetContentAnalyticsResponse
        {
            EventId = request.EventId,

            PagesTotal = pageRows.Count,
            PagesPublished = pageRows.Count(x => x.IsPublished),
            PagesDraft = pageRows.Count(x => !x.IsPublished),

            PageSectionsTotal = sectionRows.Count,
            PageSectionsVisible = sectionRows.Count(x => x.IsVisible),

            SectionsByPage = pageRows
                .OrderBy(x => x.DisplayOrder)
                .Select(page => new PageSectionCountResponse(
                    page.Id,
                    page.Name,
                    sectionRows.Count(x => x.PageId == page.Id)))
                .ToList(),

            SectionsByType = sectionRows
                .GroupBy(x => x.SectionType)
                .Select(group => new KeyCountResponse(group.Key, group.Count()))
                .OrderByDescending(x => x.Count)
                .ToList(),

            NavigationItemsTotal = navRows.Count,
            NavigationItemsVisible = navRows.Count(x => x.IsVisible),

            PhotosTotal = photoRows.Count,
            PhotosVisible = photoRows.Count(x => x.IsVisible),
            PhotosPendingApproval = photoRows.Count(x => !x.IsVisible),

            FeaturesTotal = featureRows.Count,
            FeaturesEnabled = enabledFeatures.Count,
            EnabledFeatureNames = enabledFeatures
                .Select(x => x.Feature?.Name ?? string.Empty)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList()
        };
    }
}
