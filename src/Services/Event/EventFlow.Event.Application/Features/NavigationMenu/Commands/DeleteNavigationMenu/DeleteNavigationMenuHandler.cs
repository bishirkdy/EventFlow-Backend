

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Commands.DeleteNavigationMenu
{
    public sealed class DeleteNavigationMenuHandler(INavigationMenuRepository navigationMenuRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteNavigationMenuCommand>
    {
        public async Task Handle(DeleteNavigationMenuCommand request, CancellationToken cancellationToken)
        {
            // Get navigation menu
            var menu = await navigationMenuRepository.GetByIdAsync(request.Id,cancellationToken);

            // Validate menu belongs to event
            if (menu is null || menu.EventId != request.EventId)
                throw new NotFoundException("Navigation menu not found.");

            // Delete navigation menu
            navigationMenuRepository.Remove(menu);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
