using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UpdateEventPhotosVisibility;

public sealed class UpdateEventPhotosVisibilityCommandHandler : IRequestHandler<UpdateEventPhotosVisibilityCommand, int>
{
    private readonly IEventPhotoRepository _photoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventFeatureRepository _featureRepository;

    public UpdateEventPhotosVisibilityCommandHandler(IEventPhotoRepository photoRepository, IUnitOfWork unitOfWork, IEventFeatureRepository featureRepository)
    {
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
        _featureRepository = featureRepository;
    }

    public async Task<int> Handle(UpdateEventPhotosVisibilityCommand request, CancellationToken cancellationToken)
    {
        var ids = request.PhotoIds.Distinct().ToArray();

        var photos = await _photoRepository.GetByIdsAsync(ids, cancellationToken);

        if (photos.Count != ids.Length)
        {
            throw new NotFoundException("One or more photos were not found.");
        }

        if (photos.Any(photo => photo.EventId != request.EventId))
        {
            throw new NotFoundException("One or more photos do not belong to this event.");
        }

        await EventFeatureGuard.EnsureEnabledAsync(_featureRepository, request.EventId, FeatureIds.Gallery, "gallery", cancellationToken);

        foreach (var photo in photos)
        {
            if (request.IsVisible)
            {
                photo.Approve(request.ApprovedBy);
            }
            else
            {
                photo.Hide(request.ApprovedBy);
            }

            _photoRepository.Update(photo);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return photos.Count;
    }
}
