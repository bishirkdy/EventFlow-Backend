
using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Security.Authentication;
using MediatR;

namespace EventFlow.Event.Application.Features.Queries.GetEventPhotos
{
    public class GetEventPhotosQueryHandler(
        IEventPhotoRepository photoRepository,
        ICurrentUserService currentUser)
        : IRequestHandler<GetEventPhotosQuery, PaginatedResponse<GetEventPhotosResponse>>
    {
        public async Task<PaginatedResponse<GetEventPhotosResponse>> Handle(
            GetEventPhotosQuery request,
            CancellationToken cancellationToken)
        {
            // Anonymous callers may only ever see visible photos; authenticated callers may request hidden ones.
            var visibleOnly = currentUser.UserId == Guid.Empty
                ? true
                : request.VisibleOnly;

            var paged = await photoRepository.GetPagedByEventIdAsync(
                request.EventId,
                request.Page,
                request.PageSize,
                visibleOnly,
                cancellationToken);

            return new PaginatedResponse<GetEventPhotosResponse>
            {
                Items = paged.Items.Select(p => new GetEventPhotosResponse(
                    p.Id,
                    p.EventId,
                    p.PhotographerId,
                    p.ImageUrl,
                    p.ThumbnailUrl,
                    p.IsVisible,
                    p.UploadedAt,
                    p.ApprovedAt)).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }
    }
}
