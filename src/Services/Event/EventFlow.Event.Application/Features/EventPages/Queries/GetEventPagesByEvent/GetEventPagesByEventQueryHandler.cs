

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPagesByEvent
{
    public sealed class GetEventPagesByEventQueryHandler(IEventPageRepository eventPageRepository,IMapper mapper)
        : IRequestHandler<GetEventPagesByEventQuery,IReadOnlyList<GetEventPagesByEventResponse>>
    {
        public async Task<IReadOnlyList<GetEventPagesByEventResponse>> Handle(GetEventPagesByEventQuery request,CancellationToken cancellationToken)
        {
            // Get pages belonging to event
            var pages = await eventPageRepository.GetByEventIdAsync(request.EventId,cancellationToken);
            return mapper.Map<IReadOnlyList<GetEventPagesByEventResponse>>(pages);
        }
    }
}
