using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.ReorderNavigationItems
{
    public sealed class ReorderNavigationItemsHandler(
        INavigationItemRepository navigationItemRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<ReorderNavigationItemsCommand>
    {
        public async Task Handle(
            ReorderNavigationItemsCommand request,
            CancellationToken cancellationToken)
        {
            var items = await navigationItemRepository.GetByEventIdAsync(
                request.EventId,
                cancellationToken);

            if (items.Count != request.ItemIds.Count ||
                request.ItemIds.Any(id => items.All(x => x.Id != id)))
            {
                throw new NotFoundException(
                    "One or more navigation items were not found.");
            }

            for (var i = 0; i < request.ItemIds.Count; i++)
            {
                var item = items.First(x => x.Id == request.ItemIds[i]);

                item.UpdateDisplayOrder(i + 1);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}