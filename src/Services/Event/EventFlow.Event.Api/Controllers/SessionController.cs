using AutoMapper;
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
    public class SessionController(ISender sender, IMapper mapper) : ControllerBase
    {
        [HttpPost("{eventId:guid}/sessions")]
        public async Task<IActionResult> CreateSession(Guid eventId, CreateSessionRequest request, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<CreateSessionCommand>(request) with
            {
                EventId = eventId
            };

            // Send command
            var sessionId = await sender.Send(command,cancellationToken);

            var response = new ApiResponse<Guid>
            {
                Success = true,
                Message = "Session created successfully.",
                Data = sessionId
            };

            return Ok(response);
        }

        //Controller to get session under the event
        [HttpGet("{eventId:guid}/sessions")]
        public async Task<IActionResult> GetSessionsByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            // Send query
            var sessions = await sender.Send(new GetSessionsByEventQuery(eventId), cancellationToken);

            // Create API response
            var response = new ApiResponse<IReadOnlyList<GetSessionsByEventResponse>>
            {
                Success = true,
                Message = "Sessions retrieved successfully.",
                Data = sessions
            };

            return Ok(response);
        }

        //Controller to set event section sessions
        [HttpGet("{eventId:guid}/sessions/{id:guid}")]
        public async Task<IActionResult> GetSessionById(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            // Send query
            var session = await sender.Send(new GetSessionByIdQuery(id, eventId),cancellationToken);

            if (session is null)
                throw new NotFoundException("Session not found.");

            // Create API response
            var response = new ApiResponse<GetSessionByIdResponse>
            {
                Success = true,
                Message = "Session retrieved successfully.",
                Data = session
            };

            return Ok(response);
        }

        [HttpPut("{eventId:guid}/sessions/{id:guid}")]
        public async Task<IActionResult> UpdateSession(Guid eventId,Guid id, UpdateSessionRequest request, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<UpdateSessionCommand>(request) with
            {
                Id = id,
                EventId = eventId
            };

            // Send command
            await sender.Send(command, cancellationToken);

            // Create API response
            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Session updated successfully.",
                Data = null
            };

            return Ok(response);
        }

        [HttpDelete("{eventId:guid}/sessions/{id:guid}")]
        public async Task<IActionResult> DeleteSession(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            // Send command
            await sender.Send(new DeleteSessionCommand(id, eventId),cancellationToken);

            // Create API response
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
