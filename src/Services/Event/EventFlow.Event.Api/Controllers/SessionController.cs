using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.Sessions;
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
    public class SessionController(ISender sender) : ControllerBase
    {
        [HttpPost("{eventId:guid}/sessions")]
        public async Task<IActionResult> CreateSession(
            Guid eventId,
            CreateSessionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateSessionCommand(
                eventId,
                request.SectionId,
                request.Title,
                request.Description,
                request.SessionType,
                request.Capacity
            );


            var sessionId = await sender.Send(
                command,
                cancellationToken);

            var response = new ApiResponse<Guid>
            {
                Success = true,
                Message = "Session created successfully.",
                Data = sessionId
            };

            return Ok(response);
        }

        [HttpGet("{eventId:guid}/sessions")]
        public async Task<IActionResult> GetSessionsByEvent(
            Guid eventId,
            CancellationToken cancellationToken)
        {
            var sessions = await sender.Send(
                new GetSessionsByEventQuery(eventId),
                cancellationToken);

            var response =
                new ApiResponse<IReadOnlyList<GetSessionsByEventResponse>>
                {
                    Success = true,
                    Message = "Sessions retrieved successfully.",
                    Data = sessions
                };

            return Ok(response);
        }

        [HttpGet("{eventId:guid}/sessions/{id:guid}")]
        public async Task<IActionResult> GetSessionById(
            Guid eventId,
            Guid id,
            CancellationToken cancellationToken)
        {
            var session = await sender.Send(
                new GetSessionByIdQuery(id, eventId),
                cancellationToken);

            if (session is null)
                throw new NotFoundException("Session not found.");

            var response = new ApiResponse<GetSessionByIdResponse>
            {
                Success = true,
                Message = "Session retrieved successfully.",
                Data = session
            };

            return Ok(response);
        }

        [HttpPut("{eventId:guid}/sessions/{id:guid}")]
        public async Task<IActionResult> UpdateSession(
            Guid eventId,
            Guid id,
            UpdateSessionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateSessionCommand(
                id,
                eventId,
                request.Title,
                request.Description,
                request.SessionType,
                request.Capacity
            );

            await sender.Send(
                command,
                cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Session updated successfully.",
                Data = null
            };

            return Ok(response);
        }

        [HttpDelete("{eventId:guid}/sessions/{id:guid}")]
        public async Task<IActionResult> DeleteSession(
            Guid eventId,
            Guid id,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new DeleteSessionCommand(id, eventId),
                cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Session deleted successfully.",
                Data = null
            };

            return Ok(response);
        }
    }
}