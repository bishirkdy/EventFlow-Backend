using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UploadEventPhoto
{
    public sealed record UploadEventPhotoCommand(
        Guid EventId,
        Guid PhotographerId,
        string ImageUrl,
        string PublicId,
        string? ThumbnailUrl = null) : IRequest<UploadEventPhotoResponse>;

    public sealed record UploadEventPhotoResponse(Guid PhotoId, string ImageUrl, DateTime UploadedAt);
}