using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using MediatR;
namespace EventFlow.Event.Application.Features.SessionSpeakers.Commands.UnassignSpeaker;
public sealed class UnassignSpeakerCommandHandler(ISessionRepository sessionRepository, ISpeakerRepository speakerRepository, ISessionSpeakerRepository linkRepository, IEventFeatureRepository featureRepository, IUnitOfWork unitOfWork) : IRequestHandler<UnassignSpeakerCommand>
{
    public async Task Handle(UnassignSpeakerCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Speakers, "speakers", cancellationToken);
        var session = await sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null || session.EventId != request.EventId) throw new NotFoundException("Session not found for this event.");
        var links = await linkRepository.GetBySessionIdAsync(request.SessionId, cancellationToken);
        var link = links.FirstOrDefault(x => x.SpeakerId == request.SpeakerId);
        if (link is not null) { linkRepository.Remove(link); await unitOfWork.SaveChangesAsync(cancellationToken); }
    }
}
