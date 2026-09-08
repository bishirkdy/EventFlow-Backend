using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEventById
{
    public sealed record GetEventByIdQuery(Guid Id) : IRequest<GetEventByIdResponse?>;
}
