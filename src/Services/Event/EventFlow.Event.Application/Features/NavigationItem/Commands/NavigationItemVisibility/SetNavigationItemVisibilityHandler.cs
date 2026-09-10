
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.NavigationItemVisibility
{
    public sealed class SetNavigationItemVisibilityHandler(INavigationItemRepository navigationItemRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<SetNavigationItemVisibilityCommand>
    {
        public async Task Handle(SetNavigationItemVisibilityCommand request,CancellationToken cancellationToken)
        {
            // Get navigation item
            var item = await navigationItemRepository.GetByIdAsync(request.Id,cancellationToken);

            // Validate item belongs to menu
            if (item is null ||
                item.NavigationMenuId != request.NavigationMenuId)
            {
                throw new NotFoundException("Navigation item not found.");
            }

            // Update visibility
            item.SetVisibility(request.IsVisible);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
