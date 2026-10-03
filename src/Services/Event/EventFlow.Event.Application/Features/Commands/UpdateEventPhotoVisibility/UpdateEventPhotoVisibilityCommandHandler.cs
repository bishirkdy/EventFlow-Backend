using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UpdateEventPhotoVisibility
{
    public class UpdateEventPhotoVisibilityCommandHandler : IRequestHandler<UpdateEventPhotoVisibilityCommand>
    {
        private readonly IEventPhotoRepository _photoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEventPhotoVisibilityCommandHandler(IEventPhotoRepository photoRepository, IUnitOfWork unitOfWork)
        {
            _photoRepository = photoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateEventPhotoVisibilityCommand request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);
            if (photo is null)
            {
                throw new NotFoundException("Photo not found.");
            }

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