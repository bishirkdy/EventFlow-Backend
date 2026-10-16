using AutoMapper;
using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents
{
    public sealed class GetAdminEventsQueryHandler(IEventRepository eventRepository, IMapper mapper)
        : IRequestHandler<GetAdminEventsQuery, PaginatedResponse<AdminEventListItemResponse>>
    {
        public async Task<PaginatedResponse<AdminEventListItemResponse>> Handle(
            GetAdminEventsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await eventRepository.GetAdminPagedAsync(
                request.Page,
                request.PageSize,
                request.Status,
                request.Search,
                cancellationToken);

            return new PaginatedResponse<AdminEventListItemResponse>
            {
                Items = mapper.Map<List<AdminEventListItemResponse>>(result.Items),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
