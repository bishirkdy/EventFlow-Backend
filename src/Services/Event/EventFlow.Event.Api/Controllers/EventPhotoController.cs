using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Commands.UploadEventPhoto;
using EventFlow.Event.Application.Features.Commands.UpdateEventPhotoVisibility;
using EventFlow.Event.Application.Features.Commands.DeleteEventPhoto;
using EventFlow.Event.Application.Features.Queries.GetEventPhotos;
using EventFlow.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/photos")]
public sealed class EventPhotoController(IMediator mediator, ICurrentUserService currentUser, IFileStorage fileStorage) : ControllerBase
{
    // POST /api/v1/events/{eventId}/photos - Upload photo (Photographer)
    [Authorize]
    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadPhoto(
        Guid eventId,
        IFormFile? image,
        CancellationToken cancellationToken)
    {
        if (image is null || image.Length == 0)
        {
            return BadRequest(ApiResponse<object?>.Fail(
                ["A photo file is required."],
                "A photo file is required.",
                HttpStatusCode.BadRequest));
        }

        StoredFile stored;

        try
        {
            stored = await fileStorage.SaveAsync(
                new UploadedFile(
                    image.OpenReadStream(),
                    image.FileName,
                    image.ContentType,
                    image.Length),
                $"events/{eventId}/photos",
                cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(ApiResponse<object?>.Fail(
                [exception.Message],
                exception.Message,
                HttpStatusCode.BadRequest));
        }

        try
        {
            var command = new UploadEventPhotoCommand(
                eventId,
                currentUser.UserId,
                stored.Url,
                stored.StorageKey);

            var response = await mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<UploadEventPhotoResponse>.Success(response, "Photo uploaded successfully."));
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

    // GET /api/v1/events/{eventId}/photos - List photos (Public for visible, Authenticated for all)
    [HttpGet]
    public async Task<IActionResult> GetPhotos(
        Guid eventId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool visibleOnly = true,
        CancellationToken cancellationToken = default)
    {
        // If requesting non-visible photos, require authentication
        if (!visibleOnly)
        {
            if (currentUser.UserId == Guid.Empty)
            {
                return Unauthorized();
            }
        }

        var query = new GetEventPhotosQuery(eventId, page, pageSize, visibleOnly);
        var response = await mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<PaginatedResponse<GetEventPhotosResponse>>.Success(response, "Photos retrieved successfully."));
    }

    // PATCH /api/v1/events/{eventId}/photos/{photoId}/visibility - Approve/hide photo (Organizer)
    [Authorize]
    [HttpPatch("{photoId:guid}/visibility")]
    public async Task<IActionResult> UpdateVisibility(
        Guid eventId,
        Guid photoId,
        [FromBody] UpdateVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEventPhotoVisibilityCommand(photoId, request.IsVisible, currentUser.UserId);
        await mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, request.IsVisible ? "Photo approved." : "Photo hidden."));
    }

    // DELETE /api/v1/events/{eventId}/photos/{photoId} - Delete photo (Photographer or Organizer)
    [Authorize]
    [HttpDelete("{photoId:guid}")]
    public async Task<IActionResult> DeletePhoto(
        Guid eventId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteEventPhotoCommand(photoId, currentUser.UserId);
        await mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, "Photo deleted successfully."));
    }

    public sealed record UpdateVisibilityRequest(bool IsVisible);
}