
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenusByEvent
{
    public sealed record GetNavigationMenusByEventQuery(Guid EventId): IRequest<IReadOnlyList<GetNavigationMenusByEventResponse>>;
}
