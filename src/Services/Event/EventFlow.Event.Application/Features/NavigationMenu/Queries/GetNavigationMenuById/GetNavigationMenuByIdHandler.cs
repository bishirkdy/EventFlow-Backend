

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenuById
{
    public sealed class GetNavigationMenuByIdHandler(
        INavigationMenuRepository navigationMenuRepository,IMapper mapper)
        : IRequestHandler<GetNavigationMenuByIdQuery, GetNavigationMenuByIdResponse>
    {
        public async Task<GetNavigationMenuByIdResponse> Handle(GetNavigationMenuByIdQuery request,CancellationToken cancellationToken)
        {
            // Get navigation menu
            var menu = await navigationMenuRepository.GetByIdAsync(request.Id, cancellationToken);

            // Validate menu belongs to event
            if (menu is null || menu.EventId != request.EventId)
                throw new NotFoundException("Navigation menu not found.");

            // Map entity to response
            return mapper.Map<GetNavigationMenuByIdResponse>(menu);
        }
    }
}
