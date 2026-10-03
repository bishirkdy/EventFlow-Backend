using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Commands.DeleteEventPhoto
{
    public class DeleteEventPhotoCommandHandler : IRequestHandler<DeleteEventPhotoCommand>
    {
        private readonly IEventPhotoRepository _photoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEventPhotoCommandHandler(IEventPhotoRepository photoRepository, IUnitOfWork unitOfWork)
        {
            _photoRepository = photoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteEventPhotoCommand request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);
            if (photo is null)
            {
                throw new NotFoundException("Photo not found.");
            }

            // Check if user is the photographer or organizer (could add role check here)
            if (photo.PhotographerId != request.RequestedBy)
            {
                // Could add authorization check for organizer role
                // For now, allow photographer to delete their own photos
                throw new ForbiddenException("You can only delete your own photos.");
            }

            _photoRepository.Remove(photo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}