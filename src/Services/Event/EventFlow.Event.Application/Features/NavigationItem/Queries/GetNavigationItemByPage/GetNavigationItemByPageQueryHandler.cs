
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItemByPage
{
    public sealed class GetNavigationItemByPageQueryHandler(INavigationItemRepository navigationItemRepository):
        IRequestHandler<GetNavigationItemByPageQuery,GetNavigationItemByPageResponse?>
    {
        public async Task<GetNavigationItemByPageResponse?> Handle(GetNavigationItemByPageQuery request,CancellationToken cancellationToken)
        {
            var item = await navigationItemRepository.GetByPageIdAsync(request.PageId,cancellationToken);

            if (item is null)
                return null;

            return new GetNavigationItemByPageResponse(
                item.Id,
                item.NavigationMenuId,
                item.NavigationMenu.Name,
                item.Label,
                item.PageId!.Value,
                item.DisplayOrder,
                item.IsVisible,
                item.OpenInNewTab);
        }
    }
}
