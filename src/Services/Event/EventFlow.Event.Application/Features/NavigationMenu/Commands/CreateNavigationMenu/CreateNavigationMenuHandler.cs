using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Commands.CreateNavigationMenu
{
    public sealed class CreateNavigationMenuHandler(INavigationMenuRepository navigationMenuRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateNavigationMenuCommand, Guid>
    {
        public async Task<Guid> Handle(CreateNavigationMenuCommand request, CancellationToken cancellationToken)
        {
            // Create navigation menu
            var menu = new Domain.Entities.NavigationMenu(
                request.EventId,
                request.Name,
                request.Location);

            await navigationMenuRepository.AddAsync(menu,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return menu.Id;
        }
    }
}
