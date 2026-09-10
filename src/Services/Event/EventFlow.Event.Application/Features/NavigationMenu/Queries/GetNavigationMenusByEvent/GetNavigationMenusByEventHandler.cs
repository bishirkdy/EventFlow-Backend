

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenusByEvent
{
    public sealed class GetNavigationMenusByEventHandler(INavigationMenuRepository navigationMenuRepository,IMapper mapper)
        : IRequestHandler<GetNavigationMenusByEventQuery, IReadOnlyList<GetNavigationMenusByEventResponse>>
    {
        public async Task<IReadOnlyList<GetNavigationMenusByEventResponse>> Handle(GetNavigationMenusByEventQuery request, CancellationToken cancellationToken)
        {
            // Get navigation menus belonging to event
            var menus = await navigationMenuRepository.GetByEventIdAsync(request.EventId,cancellationToken);

            // Map entities to responses
            return mapper.Map<IReadOnlyList<GetNavigationMenusByEventResponse>>(menus);
        }
    }
}
