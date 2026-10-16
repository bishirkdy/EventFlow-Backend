using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UpdateEventPhotoVisibility
{
    public class UpdateEventPhotoVisibilityCommandHandler : IRequestHandler<UpdateEventPhotoVisibilityCommand>
    {
        private readonly IEventPhotoRepository _photoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventFeatureRepository _featureRepository;

        public UpdateEventPhotoVisibilityCommandHandler(IEventPhotoRepository photoRepository, IUnitOfWork unitOfWork, IEventFeatureRepository featureRepository)
        {
            _photoRepository = photoRepository;
            _unitOfWork = unitOfWork;
            _featureRepository = featureRepository;
        }

        public async Task Handle(UpdateEventPhotoVisibilityCommand request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);
            if (photo is null)
            {
                throw new NotFoundException("Photo not found.");
            }

            await EventFeatureGuard.EnsureEnabledAsync(_featureRepository, photo.EventId, FeatureIds.Gallery, "gallery", cancellationToken);

            if (request.IsVisible)
            {
                photo.Approve(request.ApprovedBy);
            }
            else
            {
                photo.Hide(request.ApprovedBy);
            }

            _photoRepository.Update(photo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}