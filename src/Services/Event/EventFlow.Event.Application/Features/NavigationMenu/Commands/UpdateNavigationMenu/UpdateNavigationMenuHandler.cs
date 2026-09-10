
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Commands.UpdateNavigationMenu
{
    public sealed class UpdateNavigationMenuHandler(INavigationMenuRepository navigationMenuRepository,
        IUnitOfWork unitOfWork): IRequestHandler<UpdateNavigationMenuCommand>
    {
        public async Task Handle(UpdateNavigationMenuCommand request,CancellationToken cancellationToken)
        {
            // Get navigation menu
            var menu = await navigationMenuRepository.GetByIdAsync(request.Id, cancellationToken);

            // Validate menu belongs to event
            if (menu is null || menu.EventId != request.EventId)
                throw new NotFoundException("Navigation menu not found.");

            // Update navigation menu
            menu.Update(request.Name,request.Location);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
