using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem
{
    public sealed class CreateNavigationItemHandler(
        INavigationItemRepository navigationItemRepository,
        IEventPageRepository eventPageRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateNavigationItemCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateNavigationItemCommand request,
            CancellationToken cancellationToken)
        {
            var page = await eventPageRepository.GetByIdAsync(
                request.PageId,
                cancellationToken);

            if (page is null || page.EventId != request.EventId)
                throw new NotFoundException(
                    "Navigation page not found for this event.");

            var existingItems = await navigationItemRepository.GetByEventIdAsync(
                request.EventId,
                cancellationToken);

            var displayOrder = existingItems.Count == 0
                ? 1
                : existingItems.Max(x => x.DisplayOrder) + 1;

            var item = new Domain.Entities.NavigationItem(
                request.EventId,
                request.Label,
                request.PageId,
                displayOrder);

            await navigationItemRepository.AddAsync(
                item,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}