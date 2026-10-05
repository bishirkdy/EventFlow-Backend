namespace EventFlow.Event.Application.Features.Queries.GetEventPhotos;

public sealed record GetEventPhotosResponse(
    Guid PhotoId,
    Guid EventId,
    Guid PhotographerId,
    string ImageUrl,
    string? ThumbnailUrl,
    bool IsVisible,
    DateTime UploadedAt,
    DateTime? ApprovedAt);
