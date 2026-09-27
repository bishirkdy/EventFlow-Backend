using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.ReorderEventPages
{
    public sealed record ReorderEventPagesCommand(
        Guid EventId,
        IReadOnlyList<Guid> PageIds) : IRequest;
}
