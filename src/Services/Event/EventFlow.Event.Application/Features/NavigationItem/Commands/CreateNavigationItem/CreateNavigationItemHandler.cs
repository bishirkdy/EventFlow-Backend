using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;
namespace EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem
{
    public sealed class CreateNavigationItemHandler(INavigationItemRepository navigationItemRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<CreateNavigationItemCommand, Guid>
    {
        public async Task<Guid> Handle(CreateNavigationItemCommand request,CancellationToken cancellationToken)
        {
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
