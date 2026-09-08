using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Application.Features.Events.Commands.CreateEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    [Authorize]
    public sealed class EventsController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateEventCommand request, CancellationToken cancellationToken)
        {

            var command = new CreateEventCommand(
                request.Name,
                request.Description,
                request.EventType,
                request.SubType,
                request.StartDate,
                request.EndDate,
                request.TimeZone);

            var result = await sender.Send(
                command,
                cancellationToken);

            var response = new CreateEventResponse(result.Id);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<CreateEventResponse>
                {
                    Success = true,
                    Message = "Event created successfully.",
                    Data = response
                });
        }


        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            // We will implement GetEventByIdQuery next.

            return Ok();
        }
    }
}
