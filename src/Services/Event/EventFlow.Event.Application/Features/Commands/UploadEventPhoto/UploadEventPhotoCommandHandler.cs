using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Commands.UploadEventPhoto;
using EventFlow.Event.Domain.Entities;
using EventFlow.Security.Authentication;
using FluentValidation;
using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UploadEventPhoto;

public sealed class UploadEventPhotoCommandHandler(
    IEventPhotoRepository photoRepository,
    IUnitOfWork unitOfWork,
    IFileStorage fileStorage,
    ICurrentUserService currentUser)
    : IRequestHandler<UploadEventPhotoCommand, UploadEventPhotoResponse>
{
    public async Task<UploadEventPhotoResponse> Handle(
        UploadEventPhotoCommand request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            throw new ValidationException("A photo file is required.");
        }

        StoredFile stored;

        try
        {
            stored = await fileStorage.SaveAsync(
                request.File,
                $"events/{request.EventId}/photos",
                cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            throw new ValidationException(exception.Message);
        }

        try
        {
            var photo = new EventPhoto(
                request.EventId,
                currentUser.UserId,
                stored.Url,
                stored.StorageKey,
                null);

            await photoRepository.AddAsync(photo, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new UploadEventPhotoResponse(photo.Id, photo.ImageUrl, photo.UploadedAt);
        }
        catch
        {
            // Do not leave orphaned files in storage when the record fails.
            try
            {
                await fileStorage.DeleteAsync(stored.StorageKey, cancellationToken);
            }
            catch
            {
                // Best effort cleanup only.
            }

            throw;
        }
    }
}
