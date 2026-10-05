using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventType.Queries.GetActiveEventTypes;

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