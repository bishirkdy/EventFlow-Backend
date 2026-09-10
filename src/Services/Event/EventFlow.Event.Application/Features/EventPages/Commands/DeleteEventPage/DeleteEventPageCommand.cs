

using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.DeleteEventPage
{
    public sealed record DeleteEventPageCommand(Guid EventId, Guid Id) : IRequest;
}
