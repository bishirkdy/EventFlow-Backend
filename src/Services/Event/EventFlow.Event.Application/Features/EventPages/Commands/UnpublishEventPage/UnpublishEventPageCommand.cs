
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.UnpublishEventPage
{
    public sealed record UnpublishEventPageCommand(Guid EventId, Guid Id) : IRequest;
}
