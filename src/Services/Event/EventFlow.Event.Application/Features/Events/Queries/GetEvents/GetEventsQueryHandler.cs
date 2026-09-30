using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Contracts.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEvents
{
    public sealed class GetEventsQueryHandler
        : IRequestHandler<GetEventsQuery, PaginatedResponse<GetEventResponse>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public GetEventsQueryHandler(IEventRepository eventRepository , IMapper mapper)
        {
            _eventRepository = eventRepository ;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<GetEventResponse>> Handle(
            GetEventsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _eventRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                cancellationToken);

            var items = _mapper.Map<List<GetEventResponse>>(result.Items);


            return new PaginatedResponse<GetEventResponse>
            {
                Items = items,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
