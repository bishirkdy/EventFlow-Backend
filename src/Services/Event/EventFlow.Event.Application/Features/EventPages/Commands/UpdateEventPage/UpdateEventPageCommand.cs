

using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.UpdateEventPage
{
    public sealed record UpdateEventPageCommand(
        Guid Id,
        Guid EventId,
        string Name,
        string Slug,
        string PageType,
        int DisplayOrder) : IRequest;
}
