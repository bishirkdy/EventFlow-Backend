using EventFlow.Contracts.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Queries.GetEventPhotos
{
    public sealed record GetEventPhotosQuery(Guid EventId, int Page = 1, int PageSize = 20, bool VisibleOnly = true) : IRequest<PaginatedResponse<GetEventPhotosResponse>>;

    public sealed record GetEventPhotosResponse(
        Guid PhotoId,
        Guid EventId,
        Guid PhotographerId,
        string ImageUrl,
        string? ThumbnailUrl,
        bool IsVisible,
        DateTime UploadedAt,
        DateTime? ApprovedAt);
}