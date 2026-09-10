
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.PublishEventPage
{
    public sealed record PublishEventPageCommand(Guid EventId, Guid Id) : IRequest;
}
