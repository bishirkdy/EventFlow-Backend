using MediatR;


namespace EventFlow.Event.Application.Features.EventType.Queries.GetActiveEventTypes
{
    public sealed record GetActiveEventTypesQuery : IRequest<IReadOnlyList<GetEventTypeResponse>>;
}
