using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Enums;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEventIsPublished;

public sealed class GetEventIsPublishedQueryHandler(IEventRepository eventRepository)
    : IRequestHandler<GetEventIsPublishedQuery, bool>
{
    public async Task<bool> Handle(
        GetEventIsPublishedQuery request,
        CancellationToken cancellationToken)
    {
        var eventEntity = await eventRepository.GetByIdAsync(
            request.EventId,
            cancellationToken);

        return eventEntity?.Status == EventStatus.Published;
    }
}
