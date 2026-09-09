
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenuesByEvent
{
    public sealed record GetVenuesByEventQuery(Guid EventId): IRequest<IReadOnlyList<GetVenuesByEventResponse>>;
}
