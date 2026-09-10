
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenuById
{
    public sealed record GetNavigationMenuByIdQuery(
        Guid EventId,
        Guid Id)
        : IRequest<GetNavigationMenuByIdResponse>;
}
