

using EventFlow.Event.Application.Common.Models;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEvents
{
    public sealed record GetEventsQuery(
        int Page = 1,
        int PageSize = 10
    ) : IRequest<PaginatedResult<EventResponse>>;
}
