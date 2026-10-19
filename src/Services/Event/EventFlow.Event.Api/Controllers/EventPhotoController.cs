using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.EventPhotos;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Commands.DeleteEventPhoto;
using EventFlow.Event.Application.Features.Commands.UpdateEventPhotoVisibility;
using EventFlow.Event.Application.Features.Commands.UpdateEventPhotosVisibility;
using EventFlow.Event.Application.Features.Commands.UploadEventPhoto;
using EventFlow.Event.Application.Features.Queries.GetEventPhotos;
using EventFlow.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/photos")]
public sealed class EventPhotoController(ISender sender, ICurrentUserService currentUser, IEventPreviewAccess previewAccess) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadPhoto(Guid eventId, IFormFile? image, CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new UploadEventPhotoCommand(
                eventId,
                image is null
                    ? null
                    : new UploadedFile(
                        image.OpenReadStream(),
                        image.FileName,
                        image.ContentType,
                        image.Length)), cancellationToken);

        return Ok(ApiResponse<UploadEventPhotoResponse>.Success(response, "Photo uploaded successfully."));
    }

    // Public gallery: the handler only returns content the caller may read,
    // so anonymous and authenticated non-members get the same empty result.
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetPhotos(Guid eventId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool? visibleOnly = null,
        CancellationToken cancellationToken = default)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
        {
            return Ok(ApiResponse<PaginatedResponse<GetEventPhotosResponse>>.Success(
                new PaginatedResponse<GetEventPhotosResponse>
                {
                    Items = [],
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalCount = 0
                },
                "Photos retrieved successfully."));
        }

        var response = await sender.Send(
            new GetEventPhotosQuery(eventId, page, pageSize, visibleOnly),
            cancellationToken);

        return Ok(ApiResponse<PaginatedResponse<GetEventPhotosResponse>>.Success(response, "Photos retrieved successfully."));
    }

    [Authorize]
    [HttpPatch("{photoId:guid}/visibility")]
    public async Task<IActionResult> UpdateVisibility(Guid eventId, Guid photoId, [FromBody] UpdateVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEventPhotoVisibilityCommand(photoId, request.IsVisible, currentUser.UserId);

        await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, request.IsVisible ? "Photo approved." : "Photo hidden."));
    }

    [Authorize]
    [HttpPatch("visibility")]
    public async Task<IActionResult> UpdateVisibilityBulk(Guid eventId, [FromBody] UpdatePhotosVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEventPhotosVisibilityCommand(eventId, request.PhotoIds, request.IsVisible, currentUser.UserId);

        var updated = await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, request.IsVisible ? $"Approved {updated} photo(s)." : $"Hidden {updated} photo(s)."));
    }

    [Authorize]
    [HttpDelete("{photoId:guid}")]
    public async Task<IActionResult> DeletePhoto(Guid eventId, Guid photoId, CancellationToken cancellationToken)
    {
        var command = new DeleteEventPhotoCommand(photoId, currentUser.UserId);

        await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, "Photo deleted successfully."));
    }
}
