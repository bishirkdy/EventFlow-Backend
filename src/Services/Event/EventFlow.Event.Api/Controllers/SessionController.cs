using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Sessions;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Application.Features.Sessions.Commands.CreateSession;
using EventFlow.Event.Application.Features.Sessions.Commands.DeleteSession;
using EventFlow.Event.Application.Features.Sessions.Commands.UpdateSession;
using EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById;
using EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/session")]
    [Authorize]
    public sealed class SessionController(ISender sender) : ControllerBase
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
                request.Capacity, request.StartTime, request.EndTime, request.VenueId, image), cancellationToken);

            return Ok(new ApiResponse<Guid> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Session created successfully.", Data = sessionId });
        }

        [AllowAnonymous]
        [HttpGet("{eventId:guid}/sessions")]
        public async Task<IActionResult> GetSessionsByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            var sessions = await sender.Send(new GetSessionsByEventQuery(eventId), cancellationToken);
            return Ok(new ApiResponse<IReadOnlyList<GetSessionsByEventResponse>> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Sessions retrieved successfully.", Data = sessions });
        }

        [AllowAnonymous]
        [HttpGet("{eventId:guid}/sessions/{id:guid}")]
        public async Task<IActionResult> GetSessionById(Guid eventId, Guid id, CancellationToken cancellationToken)
        {
            var session = await sender.Send(new GetSessionByIdQuery(id, eventId), cancellationToken);
            if (session is null) throw new NotFoundException("Session not found.");
            return Ok(new ApiResponse<GetSessionByIdResponse> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Session retrieved successfully.", Data = session });
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
                request.StartTime, request.EndTime, request.VenueId, image), cancellationToken);

            return Ok(new ApiResponse<object?> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Session updated successfully.", Data = null });
        }

        [HttpDelete("{eventId:guid}/sessions/{id:guid}")]
        public async Task<IActionResult> DeleteSession(Guid eventId, Guid id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteSessionCommand(id, eventId), cancellationToken);
            return Ok(new ApiResponse<object?> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Session deleted successfully.", Data = null });
        }
    }
}
