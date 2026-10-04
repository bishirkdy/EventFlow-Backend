using EventFlow.Event.Application.Abstractions.Authorization;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Commands.DeleteEventPhoto
{
    public class DeleteEventPhotoCommandHandler : IRequestHandler<DeleteEventPhotoCommand>
    {
        private readonly IEventPhotoRepository _photoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFileStorage _fileStorage;

        public DeleteEventPhotoCommandHandler(
            IEventPhotoRepository photoRepository,
            IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            IFileStorage fileStorage)
        {
            _photoRepository = photoRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _fileStorage = fileStorage;
        }

        public async Task Handle(DeleteEventPhotoCommand request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);
            if (photo is null)
            {
                throw new NotFoundException("Photo not found.");
            }

            if (photo.PhotographerId != request.RequestedBy)
            {
                // Organizers/owners (event.update) may delete any photo of the event.
                var canManageAnyPhoto = await _authorizationService.HasPermissionAsync(
                    request.RequestedBy,
                    photo.EventId,
                    "event.update",
                    cancellationToken);

                if (!canManageAnyPhoto)
                {
                    throw new ForbiddenException("You can only delete your own photos.");
                }
            }

            _photoRepository.Remove(photo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(photo.PublicId))
            {
                try
                {
                    await _fileStorage.DeleteAsync(photo.PublicId, cancellationToken);
                }
                catch
                {
                    // Storage cleanup must not fail the deletion request.
                }
            }
        }
    }
}
