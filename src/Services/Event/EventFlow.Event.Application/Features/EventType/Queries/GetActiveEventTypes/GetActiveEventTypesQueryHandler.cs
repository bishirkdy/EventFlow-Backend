using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Features.EventType.Queries.GetActiveEventTypes;
using MediatR;

namespace EventFlow.Event.Application.EventTypes.Queries.GetActiveEventTypes;

public sealed class GetActiveEventTypesQueryHandler(IEventTypeRepository eventTypeRepository)
    : IRequestHandler<GetActiveEventTypesQuery, IReadOnlyList<GetEventTypeResponse>>
{
    public async Task<IReadOnlyList<GetEventTypeResponse>> Handle(
        GetActiveEventTypesQuery request, CancellationToken cancellationToken)
    {
        var eventTypes = await eventTypeRepository
            .GetActiveAsync(cancellationToken);

        return eventTypes
            .Select(x => new GetEventTypeResponse(
                x.Id,
                x.Code,
                x.Name,
                x.Description))
            .ToList();
    }
}