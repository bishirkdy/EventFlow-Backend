using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventFlow.Event.Application.Features.Events.Queries.GetPublicEventStats;

public sealed class GetPublicEventStatsQueryHandler(
    IEventRepository eventRepository,
    IRegistrationClient registrationClient,
    ILogger<GetPublicEventStatsQueryHandler> logger)
    : IRequestHandler<GetPublicEventStatsQuery, GetPublicEventStatsResponse>
{
    public async Task<GetPublicEventStatsResponse> Handle(
        GetPublicEventStatsQuery request,
        CancellationToken cancellationToken)
    {
        var publishedEvents = await eventRepository.CountPublishedAsync(cancellationToken);

        var participants = 0;

        try
        {
            participants = await registrationClient.GetParticipantCountAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // The landing page should still render when registration is unavailable.
            logger.LogWarning(exception, "Failed to load the public participant count.");
        }

        return new GetPublicEventStatsResponse
        {
            PublishedEvents = publishedEvents,
            Participants = participants
        };
    }
}
