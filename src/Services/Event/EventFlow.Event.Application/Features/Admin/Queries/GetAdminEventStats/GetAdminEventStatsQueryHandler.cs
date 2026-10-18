using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Admin.Queries.GetAdminEventStats;

public sealed class GetAdminEventStatsQueryHandler(IEventRepository eventRepository)
    : IRequestHandler<GetAdminEventStatsQuery, GetAdminEventStatsResponse>
{
    public async Task<GetAdminEventStatsResponse> Handle(
        GetAdminEventStatsQuery request,
        CancellationToken cancellationToken)
    {
        var counts = await eventRepository.GetStatusCountsAsync(cancellationToken);

        return new GetAdminEventStatsResponse
        {
            TotalEvents = counts.TotalEvents,
            PublishedEvents = counts.PublishedEvents,
            DraftEvents = counts.DraftEvents,
            CancelledEvents = counts.CancelledEvents
        };
    }
}
