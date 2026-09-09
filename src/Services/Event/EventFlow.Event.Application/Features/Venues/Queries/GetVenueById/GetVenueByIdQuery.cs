
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenueById
{
    public sealed record GetVenueByIdQuery(Guid Id, Guid EventId): IRequest<GetVenueByIdResponse?>;
}
