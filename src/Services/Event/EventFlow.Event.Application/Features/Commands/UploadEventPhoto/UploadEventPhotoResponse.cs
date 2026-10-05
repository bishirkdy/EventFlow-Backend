namespace EventFlow.Event.Application.Features.Commands.UploadEventPhoto;

public sealed record UploadEventPhotoResponse(
    Guid PhotoId,
    string ImageUrl,
    DateTime UploadedAt);
