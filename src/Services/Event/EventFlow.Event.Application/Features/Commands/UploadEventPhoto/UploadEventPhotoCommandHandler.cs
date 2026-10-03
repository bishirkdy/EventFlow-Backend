using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UploadEventPhoto
{
    public class UploadEventPhotoCommandHandler : IRequestHandler<UploadEventPhotoCommand, UploadEventPhotoResponse>
    {
        private readonly IEventPhotoRepository _photoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UploadEventPhotoCommandHandler(IEventPhotoRepository photoRepository, IUnitOfWork unitOfWork)
        {
            _photoRepository = photoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UploadEventPhotoResponse> Handle(UploadEventPhotoCommand request, CancellationToken cancellationToken)
        {
            var photo = new EventPhoto(request.EventId, request.PhotographerId, request.ImageUrl, request.PublicId, request.ThumbnailUrl);
            await _photoRepository.AddAsync(photo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UploadEventPhotoResponse(photo.Id, photo.ImageUrl, photo.UploadedAt);
        }
    }
}