

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem
{
    public sealed class UpdateNavigationItemHandler(INavigationItemRepository navigationItemRepository, INavigationMenuRepository navigationMenuRepository, IEventPageRepository eventPageRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateNavigationItemCommand>
    {
        public async Task Handle(UpdateNavigationItemCommand request, CancellationToken cancellationToken)
        {
            // Get navigation item
            var item = await navigationItemRepository.GetByIdAsync(request.Id, cancellationToken);

            // Validate item belongs to menu
            if (item is null ||item.NavigationMenuId != request.NavigationMenuId)
            {
                throw new NotFoundException("Navigation item not found.");
            }

            var menu = await navigationMenuRepository.GetByIdAsync(request.NavigationMenuId, cancellationToken);
            if (menu is null)
                throw new NotFoundException("Navigation menu not found.");

            if (request.PageId.HasValue)
            {
                var page = await eventPageRepository.GetByIdAsync(request.PageId.Value, cancellationToken);
                if (page is null || page.EventId != menu.EventId)
                    throw new NotFoundException("Navigation page not found for this event.");
            }

            // Update navigation item
            item.Update(request.Label,request.Url,request.PageId,request.DisplayOrder,request.OpenInNewTab);

            // Update visibility
            item.SetVisibility(request.IsVisible);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
