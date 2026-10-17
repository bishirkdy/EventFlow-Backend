using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventIdByPage;

public sealed class GetEventIdByPageQueryHandler(IEventPageRepository eventPageRepository)
    : IRequestHandler<GetEventIdByPageQuery, Guid?>
{
    public async Task<Guid?> Handle(
        GetEventIdByPageQuery request,
        CancellationToken cancellationToken)
    {
        var page = await eventPageRepository.GetByIdAsync(
            request.PageId,
            cancellationToken);

        return page?.EventId;
    }
}
