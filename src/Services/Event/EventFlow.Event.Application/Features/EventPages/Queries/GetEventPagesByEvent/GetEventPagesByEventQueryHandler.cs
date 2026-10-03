using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPagesByEvent;

public sealed class GetEventPagesByEventQueryHandler(
    IEventPageRepository eventPageRepository,
    IEventRepository eventRepository,
    IMapper mapper)
    : IRequestHandler<GetEventPagesByEventQuery, IReadOnlyList<GetEventPagesByEventResponse>>
{
    public async Task<IReadOnlyList<GetEventPagesByEventResponse>> Handle(
        GetEventPagesByEventQuery request,
        CancellationToken cancellationToken)
    {
        if (!request.IncludeUnpublished)
        {
            var eventEntity = await eventRepository.GetByIdAsync(
                request.EventId,
                cancellationToken);

            if (eventEntity is null ||
                eventEntity.Status != EventFlow.Event.Domain.Enums.EventStatus.Published)
            {
                return [];
            }
        }

        var pages = await eventPageRepository.GetByEventIdAsync(
            request.EventId,
            cancellationToken);

        if (!request.IncludeUnpublished)
        {
            pages = pages.Where(page => page.IsPublished).ToList();
        }

        return mapper.Map<IReadOnlyList<GetEventPagesByEventResponse>>(pages);
    }
}
