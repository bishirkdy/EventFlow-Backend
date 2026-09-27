using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem
{
    public sealed record CreateNavigationItemCommand(
        Guid EventId,
        string Label,
        Guid PageId) : IRequest<Guid>;
}
