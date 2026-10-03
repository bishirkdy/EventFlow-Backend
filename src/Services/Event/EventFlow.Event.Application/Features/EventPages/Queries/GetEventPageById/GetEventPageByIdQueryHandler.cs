using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPageById;

public sealed class GetEventPageByIdQueryHandler(
    IEventPageRepository eventPageRepository,
    IEventRepository eventRepository,
    IMapper mapper)
    : IRequestHandler<GetEventPageByIdQuery, GetEventPageByIdResponse?>
{
    public async Task<GetEventPageByIdResponse?> Handle(
        GetEventPageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var page = await eventPageRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (page is null || page.EventId != request.EventId)
            return null;

        if (!request.IncludeUnpublished)
        {
            if (!page.IsPublished)
                return null;

            var eventEntity = await eventRepository.GetByIdAsync(
                request.EventId,
                cancellationToken);

            if (eventEntity is null ||
                eventEntity.Status != EventFlow.Event.Domain.Enums.EventStatus.Published)
                return null;
        }

        return mapper.Map<GetEventPageByIdResponse>(page);
    }
}
