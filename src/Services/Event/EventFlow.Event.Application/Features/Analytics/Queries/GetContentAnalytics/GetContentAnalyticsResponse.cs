using EventFlow.Event.Application.Features.Analytics.Common;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetContentAnalytics;

public sealed class GetContentAnalyticsResponse
{
    public Guid EventId { get; init; }

    public int PagesTotal { get; init; }

    public int PagesPublished { get; init; }

    public int PagesDraft { get; init; }

    public int PageSectionsTotal { get; init; }

    public int PageSectionsVisible { get; init; }

    public IReadOnlyList<PageSectionCountResponse> SectionsByPage { get; init; } = [];

    public IReadOnlyList<KeyCountResponse> SectionsByType { get; init; } = [];

    public int NavigationItemsTotal { get; init; }

    public int NavigationItemsVisible { get; init; }

    public int PhotosTotal { get; init; }

    public int PhotosVisible { get; init; }

    public int PhotosPendingApproval { get; init; }

    public int FeaturesTotal { get; init; }

    public int FeaturesEnabled { get; init; }

    public IReadOnlyList<string> EnabledFeatureNames { get; init; } = [];
}
