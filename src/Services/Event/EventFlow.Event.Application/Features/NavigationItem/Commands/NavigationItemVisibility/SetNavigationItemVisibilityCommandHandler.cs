
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.NavigationItemVisibility;

public sealed class SetNavigationItemVisibilityCommandHandler(INavigationItemRepository navigationItemRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<SetNavigationItemVisibilityCommand>
{
    public async Task Handle(SetNavigationItemVisibilityCommand request, CancellationToken cancellationToken)
    {
        var item = await navigationItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (item is null ||
            item.EventId != request.EventId)
        {
            throw new NotFoundException("Navigation item not found.");
        }

        item.SetVisibility(request.IsVisible);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
