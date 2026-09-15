using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common.Models;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEvents
{
    public sealed class GetEventsQueryHandler
        : IRequestHandler<GetEventsQuery, PaginatedResult<GetEventResponse>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public GetEventsQueryHandler(IEventRepository eventRepository , IMapper mapper)
        {
            _eventRepository = eventRepository ;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetEventResponse>> Handle(
            GetEventsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _eventRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                cancellationToken);

            var items = _mapper.Map<List<GetEventResponse>>(result.Items);


            return new PaginatedResult<GetEventResponse>
            {
                Items = items,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
