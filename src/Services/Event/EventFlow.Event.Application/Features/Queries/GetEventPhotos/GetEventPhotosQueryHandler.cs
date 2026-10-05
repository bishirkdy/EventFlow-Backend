using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
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
            if (!request.VisibleOnly && currentUser.UserId == Guid.Empty)
            {
                throw new UnauthorizedException(
                    "Authentication is required to view non-public photos.");
            }

            var paged = await photoRepository.GetPagedByEventIdAsync(
                request.EventId,
                request.Page,
                request.PageSize,
                request.VisibleOnly,
                cancellationToken);

            var response = new PaginatedResponse<GetEventPhotosResponse>
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

            return response;
        }
    }
}
