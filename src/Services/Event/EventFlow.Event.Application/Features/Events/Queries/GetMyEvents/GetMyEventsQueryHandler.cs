using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents
{
    // Query Handler
    public sealed class GetMyEventsQueryHandler(IEventRepository eventRepository,IMapper mapper): IRequestHandler<GetMyEventsQuery, IReadOnlyList<GetMyEventsResponse>>
    {
        public async Task<IReadOnlyList<GetMyEventsResponse>> Handle(GetMyEventsQuery request, CancellationToken cancellationToken)
        {
            // Get events created by the user
            var events = await eventRepository.GetByCreatedByAsync(request.UserId, cancellationToken);

            // Map entities to response models
            return mapper.Map<IReadOnlyList<GetMyEventsResponse>>(events);
        }
    }
}
