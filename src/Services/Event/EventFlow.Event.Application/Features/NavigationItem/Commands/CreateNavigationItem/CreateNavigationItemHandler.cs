using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;
namespace EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem
{
    public sealed class CreateNavigationItemHandler(INavigationItemRepository navigationItemRepository, INavigationMenuRepository navigationMenuRepository, IEventPageRepository eventPageRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateNavigationItemCommand, Guid>
    {
        public async Task<Guid> Handle(CreateNavigationItemCommand request,CancellationToken cancellationToken)
        {
            var menu = await navigationMenuRepository.GetByIdAsync(request.NavigationMenuId, cancellationToken);
            if (menu is null)
                throw new NotFoundException("Navigation menu not found.");

            if (request.PageId.HasValue)
            {
                var page = await eventPageRepository.GetByIdAsync(request.PageId.Value, cancellationToken);
                if (page is null || page.EventId != menu.EventId)
                    throw new NotFoundException("Navigation page not found for this event.");
            }

            // Create navigation item
            var item = new Domain.Entities.NavigationItem(
                request.NavigationMenuId,
                request.Label,
                request.Url,
                request.PageId,
                request.DisplayOrder,
                request.OpenInNewTab);

            // Save navigation item
            await navigationItemRepository.AddAsync(item,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}
