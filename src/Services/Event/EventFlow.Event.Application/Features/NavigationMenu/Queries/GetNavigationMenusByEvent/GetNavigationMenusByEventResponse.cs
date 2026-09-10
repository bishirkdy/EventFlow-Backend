

namespace EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenusByEvent
{
    public sealed record GetNavigationMenusByEventResponse(
       Guid Id,
       Guid EventId,
       string Name,
       string Location,
       DateTime CreatedAt,
       DateTime? UpdatedAt);
}
