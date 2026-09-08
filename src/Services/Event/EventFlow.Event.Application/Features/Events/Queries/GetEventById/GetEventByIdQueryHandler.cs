using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEventById
{
    //Query to get event by id
    public sealed class GetEventByIdQueryHandler(IEventRepository eventRepository , IMapper mapper): IRequestHandler<GetEventByIdQuery, GetEventByIdResponse?>
    {
        public async Task<GetEventByIdResponse?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var eventEntity = await eventRepository.GetByIdAsync(request.Id,cancellationToken);

            if (eventEntity is null)
                return null;

            return mapper.Map<GetEventByIdResponse>(eventEntity);
        }
    }
}
