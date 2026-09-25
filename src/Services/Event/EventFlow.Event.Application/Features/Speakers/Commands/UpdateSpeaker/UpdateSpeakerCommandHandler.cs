using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Speakers.Commands.UpdateSpeaker;

public sealed class UpdateSpeakerCommandHandler(ISpeakerRepository repository, IEventFeatureRepository featureRepository, IFileStorage fileStorage, IUnitOfWork unitOfWork) : IRequestHandler<UpdateSpeakerCommand>
{
    public async Task Handle(UpdateSpeakerCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Speakers, "speakers", cancellationToken);
        var speaker = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (speaker is null || speaker.EventId != request.EventId) throw new NotFoundException("Speaker not found.");
        var imageUrl = speaker.ImageUrl;
        if (request.Image is not null)
            imageUrl = (await fileStorage.SaveAsync(request.Image, $"events/{request.EventId:D}/speakers", cancellationToken)).Url;
        speaker.Update(request.Name.Trim(), request.Bio, request.Designation, request.Organization, request.Email, imageUrl, request.DisplayOrder, request.IsActive);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
