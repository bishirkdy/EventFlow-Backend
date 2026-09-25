using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using MediatR;
namespace EventFlow.Event.Application.Features.SessionSpeakers.Queries.GetSessionSpeakers;
public sealed class GetSessionSpeakersQueryHandler(ISessionRepository sessionRepository, ISessionSpeakerRepository linkRepository, IEventFeatureRepository featureRepository) : IRequestHandler<GetSessionSpeakersQuery, IReadOnlyList<GetSessionSpeakersResponse>>
{
    public async Task<IReadOnlyList<GetSessionSpeakersResponse>> Handle(GetSessionSpeakersQuery request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Speakers, "speakers", cancellationToken);
        var session = await sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null || session.EventId != request.EventId) throw new NotFoundException("Session not found for this event.");
        return (await linkRepository.GetBySessionIdAsync(request.SessionId, cancellationToken)).Select(x => new GetSessionSpeakersResponse(x.Id, x.SpeakerId, x.Speaker.Name, x.Speaker.Designation, x.Speaker.Organization, x.Speaker.ImageUrl)).ToList();
    }
}
