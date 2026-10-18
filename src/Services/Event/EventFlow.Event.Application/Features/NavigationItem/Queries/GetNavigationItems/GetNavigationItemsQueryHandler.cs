
using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Enums;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems;

public sealed class GetNavigationItemsQueryHandler(
    INavigationItemRepository navigationItemRepository,
    IEventRepository eventRepository,
    IEventPageRepository eventPageRepository,
    IMapper mapper)
    : IRequestHandler<GetNavigationItemsQuery, IReadOnlyList<GetNavigationItemsResponse>>
{
    public async Task<IReadOnlyList<GetNavigationItemsResponse>> Handle(GetNavigationItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await navigationItemRepository.GetByEventIdAsync(request.EventId, cancellationToken);

        // Public callers only ever see the navigation of a published event,
        // limited to visible items that point at published pages.
        if (!request.IncludeUnpublished)
        {
            var eventEntity = await eventRepository.GetByIdAsync(
                request.EventId,
                cancellationToken);

            if (eventEntity is null ||
                eventEntity.Status != EventStatus.Published)
            {
                return [];
            }

            var pages = await eventPageRepository.GetByEventIdAsync(
                request.EventId,
                cancellationToken);

            var publishedPageIds = pages
                .Where(page => page.IsPublished)
                .Select(page => page.Id)
                .ToHashSet();

            items = items
                .Where(item => item.IsVisible && publishedPageIds.Contains(item.PageId))
                .ToList();
        }

        return mapper.Map<IReadOnlyList<GetNavigationItemsResponse>>(items);
    }
}
