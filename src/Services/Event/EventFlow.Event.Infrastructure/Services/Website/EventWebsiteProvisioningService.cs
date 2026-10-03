using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Infrastructure.Services.Website;

public sealed class EventWebsiteProvisioningService(
    IEventPageRepository pages,
    IPageSectionRepository pageSections,
    INavigationItemRepository navigation,
    IUnitOfWork unitOfWork) : IEventWebsiteProvisioningService
{
    private static readonly IReadOnlyDictionary<string, PageDefinition> Definitions =
        new Dictionary<string, PageDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["home"] = new("Home", "home", "Home", "hero", "Welcome"),
            ["schedule"] = new("Schedule", "schedule", "Schedule", "schedule", "Schedule"),
            ["venue"] = new("Venue", "venue", "Venue", "venue", "Venue"),
            ["speakers"] = new("Speakers", "speakers", "Speakers", "speakers", "Speakers"),
            ["sponsors"] = new("Sponsors", "sponsors", "Sponsors", "sponsors", "Sponsors"),
            ["gallery"] = new("Gallery", "gallery", "Gallery", "gallery", "Gallery"),
            ["registration"] = new("Registration", "register", "Registration", "rsvp", "Registration"),
        };

    public async Task EnsureInitialWebsiteAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        foreach (var resourceType in new[]
                 {
                     "home", "schedule", "venue", "speakers",
                     "sponsors", "gallery", "registration"
                 })
        {
            await EnsureResourcePageAsync(eventId, resourceType, cancellationToken);
        }
    }

    public async Task EnsureResourcePageAsync(
        Guid eventId,
        string resourceType,
        CancellationToken cancellationToken = default)
    {
        if (!Definitions.TryGetValue(resourceType, out var definition))
        {
            throw new ArgumentException(
                $"Unknown website resource type '{resourceType}'.",
                nameof(resourceType));
        }

        var existing = await pages.GetBySlugAsync(
            eventId,
            definition.Slug,
            cancellationToken);

        var page = existing;

        if (page is null)
        {
            var existingPages = await pages.GetByEventIdAsync(
                eventId,
                cancellationToken);

            var displayOrder = existingPages.Count == 0
                ? 1
                : existingPages.Max(x => x.DisplayOrder) + 1;

            page = new EventPage(
                eventId,
                definition.Name,
                definition.Slug,
                definition.PageType,
                displayOrder);

            page.Publish();
            await pages.AddAsync(page, cancellationToken);
        }

        var existingSection =
            (await pageSections.GetByPageIdAsync(page.Id, includeUnpublished: true, cancellationToken: cancellationToken))
            .FirstOrDefault(x =>
                x.SectionType.Equals(
                    definition.SectionType,
                    StringComparison.OrdinalIgnoreCase));

        if (existingSection is null)
        {
            var sections = await pageSections.GetByPageIdAsync(
                page.Id,
                cancellationToken);

            var displayOrder = sections.Count == 0
                ? 1
                : sections.Max(x => x.DisplayOrder) + 1;

            await pageSections.AddAsync(
                new PageSection(
                    page.Id,
                    definition.SectionType,
                    definition.SectionTitle,
                    null,
                    null,
                    null,
                    displayOrder,
                    null),
                cancellationToken);
        }

        var existingNavigation = await navigation.GetByPageIdAsync(
            page.Id,
            cancellationToken);

        if (existingNavigation is null)
        {
            var items = await navigation.GetByEventIdAsync(
                eventId,
                cancellationToken);

            var displayOrder = items.Count == 0
                ? 1
                : items.Max(x => x.DisplayOrder) + 1;

            await navigation.AddAsync(
                new NavigationItem(
                    eventId,
                    definition.Name,
                    page.Id,
                    displayOrder),
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private sealed record PageDefinition(
        string Name,
        string Slug,
        string PageType,
        string SectionType,
        string SectionTitle);
}
