

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPageById
{
    public sealed class GetEventPageByIdQueryHandler(IEventPageRepository eventPageRepository , IMapper mapper) : IRequestHandler<GetEventPageByIdQuery, GetEventPageByIdResponse?>
    {
        public async Task<GetEventPageByIdResponse?> Handle(GetEventPageByIdQuery request,CancellationToken cancellationToken)
        {
            var page = await eventPageRepository.GetByIdAsync(request.Id,cancellationToken);

            if (page is null || page.EventId != request.EventId)
            {
                return null;
            }

            return mapper.Map<GetEventPageByIdResponse>(page);
        }
    }
}
