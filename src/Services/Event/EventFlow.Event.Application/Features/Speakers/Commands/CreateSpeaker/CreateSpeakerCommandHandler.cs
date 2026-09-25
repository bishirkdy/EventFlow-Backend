using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Domain.Entities;
using MediatR;

namespace EventFlow.Event.Application.Features.Speakers.Commands.CreateSpeaker;

public sealed class CreateSpeakerCommandHandler(ISpeakerRepository repository, IEventFeatureRepository featureRepository, IFileStorage fileStorage, IUnitOfWork unitOfWork) : IRequestHandler<CreateSpeakerCommand, Guid>
{
    public async Task<Guid> Handle(CreateSpeakerCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Speakers, "speakers", cancellationToken);
        string? imageUrl = null;
        if (request.Image is not null)
            imageUrl = (await fileStorage.SaveAsync(request.Image, $"events/{request.EventId:D}/speakers", cancellationToken)).Url;
        var speaker = new Speaker(request.EventId, request.Name.Trim(), request.Bio, request.Designation, request.Organization, request.Email, imageUrl, request.DisplayOrder);
        await repository.AddAsync(speaker, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return speaker.Id;
    }
}
