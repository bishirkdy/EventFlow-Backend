using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using MediatR;
namespace EventFlow.Event.Application.Features.Speakers.Commands.DeleteSpeaker;
public sealed class DeleteSpeakerCommandHandler(ISpeakerRepository repository, IEventFeatureRepository featureRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteSpeakerCommand>
{
    public async Task Handle(DeleteSpeakerCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Speakers, "speakers", cancellationToken);
        var speaker = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (speaker is null || speaker.EventId != request.EventId) throw new NotFoundException("Speaker not found.");
        repository.Remove(speaker);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
