

namespace EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenuById
{
    public sealed record GetNavigationMenuByIdResponse(
        Guid Id,
        Guid EventId,
        string Name,
        string Location,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
