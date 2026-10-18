using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Enums;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItemByPage;

public sealed class GetNavigationItemByPageQueryHandler(
    INavigationItemRepository navigationItemRepository,
    IEventPageRepository eventPageRepository,
    IEventRepository eventRepository)
    : IRequestHandler<GetNavigationItemByPageQuery, GetNavigationItemByPageResponse?>
{
    public async Task<GetNavigationItemByPageResponse?> Handle(
        GetNavigationItemByPageQuery request,
        CancellationToken cancellationToken)
    {
        var item = await navigationItemRepository.GetByPageIdAsync(
            request.PageId,
            cancellationToken);

        if (item is null)
            return null;

        // Public callers only see visible items whose page belongs to a
        // published event.
        if (!request.IncludeUnpublished)
        {
            if (!item.IsVisible)
                return null;

            var page = await eventPageRepository.GetByIdAsync(
                request.PageId,
                cancellationToken);

            if (page is null || !page.IsPublished)
                return null;

            var eventEntity = await eventRepository.GetByIdAsync(
                page.EventId,
                cancellationToken);

            if (eventEntity is null ||
                eventEntity.Status != EventStatus.Published)
            {
                return null;
            }
        }

        return new GetNavigationItemByPageResponse(
            item.Id,
            item.EventId,
            item.Label,
            item.PageId,
            item.DisplayOrder,
            item.IsVisible);
    }
}
