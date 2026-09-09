

using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands
{
    public sealed record CreateEventPageCommand(
        Guid EventId,
        string Name,
        string Slug,
        string PageType,
        int DisplayOrder) : IRequest<Guid>;
}
