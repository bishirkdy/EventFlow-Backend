using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.DeleteNavigationItem
{
    public sealed class DeleteNavigationItemHandler(
        INavigationItemRepository navigationItemRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteNavigationItemCommand>
    {
        public async Task Handle(
            DeleteNavigationItemCommand request,
            CancellationToken cancellationToken)
        {
            var item = await navigationItemRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (item is null || item.EventId != request.EventId)
            {
                throw new NotFoundException("Navigation item not found.");
            }

            navigationItemRepository.Remove(item);

            var remainingItems = (await navigationItemRepository.GetByEventIdAsync(
                request.EventId,
                cancellationToken))
                .Where(x => x.Id != item.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            for (var i = 0; i < remainingItems.Count; i++)
            {
                remainingItems[i].UpdateDisplayOrder(i + 1);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}