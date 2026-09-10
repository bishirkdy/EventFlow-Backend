

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.DeleteNavigationItem
{
    public sealed class DeleteNavigationItemHandler(INavigationItemRepository navigationItemRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteNavigationItemCommand>
    {
        public async Task Handle(DeleteNavigationItemCommand request,CancellationToken cancellationToken)
        {
            // Get navigation item
            var item = await navigationItemRepository.GetByIdAsync(request.Id, cancellationToken);

            // Validate item belongs to menu
            if (item is null ||item.NavigationMenuId != request.NavigationMenuId)
            {
                throw new NotFoundException("Navigation item not found.");
            }

            // Delete navigation item
            navigationItemRepository.Remove(item);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
