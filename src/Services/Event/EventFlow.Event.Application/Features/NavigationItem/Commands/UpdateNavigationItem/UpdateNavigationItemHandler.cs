

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem
{
    public sealed class UpdateNavigationItemHandler(INavigationItemRepository navigationItemRepository,IUnitOfWork unitOfWork)
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

            // Update navigation item
            item.Update(request.Label,request.Url,request.PageId,request.DisplayOrder,request.OpenInNewTab);

            // Update visibility
            item.SetVisibility(request.IsVisible);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
