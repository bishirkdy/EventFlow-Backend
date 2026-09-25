using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Domain.Entities;
using MediatR;
namespace EventFlow.Event.Application.Features.SessionSpeakers.Commands.AssignSpeaker;
public sealed class AssignSpeakerCommandHandler(ISessionRepository sessionRepository, ISpeakerRepository speakerRepository, ISessionSpeakerRepository linkRepository, IEventFeatureRepository featureRepository, IUnitOfWork unitOfWork) : IRequestHandler<AssignSpeakerCommand>
{
    public async Task Handle(AssignSpeakerCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Speakers, "speakers", cancellationToken);
        var session = await sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        var speaker = await speakerRepository.GetByIdAsync(request.SpeakerId, cancellationToken);
        if (session is null || session.EventId != request.EventId) throw new NotFoundException("Session not found for this event.");
        if (speaker is null || speaker.EventId != request.EventId) throw new NotFoundException("Speaker not found for this event.");
        if (await linkRepository.ExistsAsync(request.SessionId, request.SpeakerId, cancellationToken)) return;
        await linkRepository.AddAsync(new SessionSpeaker(request.SessionId, request.SpeakerId), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
