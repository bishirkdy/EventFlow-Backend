

using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPageById
{
    public sealed record GetEventPageByIdQuery(
        Guid EventId,
        Guid Id) : IRequest<GetEventPageByIdResponse?>;
}
