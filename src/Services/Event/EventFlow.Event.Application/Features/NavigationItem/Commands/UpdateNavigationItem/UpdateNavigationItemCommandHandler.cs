using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem
{
    public sealed class UpdateNavigationItemCommandHandler(
        INavigationItemRepository navigationItemRepository,
        IEventPageRepository eventPageRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateNavigationItemCommand>
    {
        public async Task Handle(
            UpdateNavigationItemCommand request,
            CancellationToken cancellationToken)
        {
            var item = await navigationItemRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (item is null || item.EventId != request.EventId)
                throw new NotFoundException("Navigation item not found.");

            var page = await eventPageRepository.GetByIdAsync(
                request.PageId,
                cancellationToken);

            if (page is null || page.EventId != request.EventId)
                throw new NotFoundException("Page not found.");

            item.Update(
                request.Label,
                request.PageId);

            item.SetVisibility(request.IsVisible);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}