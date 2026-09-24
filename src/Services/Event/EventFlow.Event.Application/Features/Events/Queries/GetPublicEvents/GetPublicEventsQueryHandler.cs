using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Features.Events.Queries.GetEvents;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetPublicEvents;

public sealed class GetPublicEventsQueryHandler(
    IEventRepository eventRepository,
    IMapper mapper) : IRequestHandler<GetPublicEventsQuery, IReadOnlyList<GetEventResponse>>
{
    public async Task<IReadOnlyList<GetEventResponse>> Handle(GetPublicEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await eventRepository.GetPublishedUpcomingAsync(request.Take, cancellationToken);
        return mapper.Map<List<GetEventResponse>>(events);
    }
}
