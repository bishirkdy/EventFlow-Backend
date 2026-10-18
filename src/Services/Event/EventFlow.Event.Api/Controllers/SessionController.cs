using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Sessions;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Sessions.Commands.CreateSession;
using EventFlow.Event.Application.Features.Sessions.Commands.DeleteSession;
using EventFlow.Event.Application.Features.Sessions.Commands.UpdateSession;
using EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById;
using EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/session")]
[Authorize]
public sealed class SessionController(ISender sender, IEventPreviewAccess previewAccess) : ControllerBase
{
    [HttpPost("{eventId:guid}/sessions")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateSession(Guid eventId, [FromForm] CreateSessionRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Image?.OpenReadStream();
        var image = request.Image is null ? null : new UploadedFile(
            stream!, request.Image.FileName, request.Image.ContentType, request.Image.Length);

        var sessionId = await sender.Send(new CreateSessionCommand(
            eventId, request.SectionId, request.Title, request.Description, request.SessionType,
            request.Capacity, request.StartTime, request.EndTime, request.VenueId, request.AllowRegistration, image), cancellationToken);

        return Ok(ApiResponse<Guid>.Success(sessionId, "Session created successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/sessions")]
    public async Task<IActionResult> GetSessionsByEvent(Guid eventId, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
        {
            return Ok(ApiResponse<IReadOnlyList<GetSessionsByEventResponse>>.Success([], "Sessions retrieved successfully."));
        }

        var sessions = await sender.Send(new GetSessionsByEventQuery(eventId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<GetSessionsByEventResponse>>.Success(sessions, "Sessions retrieved successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/sessions/{id:guid}")]
    public async Task<IActionResult> GetSessionById(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
            throw new NotFoundException("Session not found.");

        var session = await sender.Send(new GetSessionByIdQuery(id, eventId), cancellationToken);
        if (session is null) throw new NotFoundException("Session not found.");
        return Ok(ApiResponse<GetSessionByIdResponse>.Success(session, "Session retrieved successfully."));
    }

    [HttpPut("{eventId:guid}/sessions/{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateSession(Guid eventId, Guid id, [FromForm] UpdateSessionRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Image?.OpenReadStream();
        var image = request.Image is null ? null : new UploadedFile(
            stream!, request.Image.FileName, request.Image.ContentType, request.Image.Length);

        await sender.Send(new UpdateSessionCommand(
            id, eventId, request.Title, request.Description, request.SessionType, request.Capacity,
            request.StartTime, request.EndTime, request.VenueId, request.AllowRegistration, image), cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, "Session updated successfully."));
    }

    [HttpDelete("{eventId:guid}/sessions/{id:guid}")]
    public async Task<IActionResult> DeleteSession(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteSessionCommand(id, eventId), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Session deleted successfully."));
    }
}
