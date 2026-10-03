using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Queries.GetEventPhotos
{
    public class GetEventPhotosQueryHandler : IRequestHandler<GetEventPhotosQuery, PaginatedResponse<GetEventPhotosResponse>>
    {
        private readonly IEventPhotoRepository _photoRepository;

        public GetEventPhotosQueryHandler(IEventPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<PaginatedResponse<GetEventPhotosResponse>> Handle(GetEventPhotosQuery request, CancellationToken cancellationToken)
        {
            var paged = await _photoRepository.GetPagedByEventIdAsync(request.EventId, request.Page, request.PageSize, request.VisibleOnly, cancellationToken);

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