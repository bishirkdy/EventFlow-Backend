using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.Events;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Application.Features.Events.Commands.CancelEvent;
using EventFlow.Event.Application.Features.Events.Commands.CreateEvent;
using EventFlow.Event.Application.Features.Events.Commands.PublishEvent;
using EventFlow.Event.Application.Features.Events.Commands.UpdateEvent;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using EventFlow.Event.Application.Features.Events.Queries.GetMyEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    [Authorize]
    public sealed class EventsController(ISender sender , IMapper mapper) : ControllerBase
    {
        //Controller for create Event
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request, CancellationToken cancellationToken)
        {
            //Map request to command
            var command = mapper.Map<CreateEventCommand>(request);

            var result = await sender.Send(command, cancellationToken);

            var response = new CreateEventResponse(result.Id);

            return StatusCode(StatusCodes.Status201Created,
                new ApiResponse<CreateEventResponse>
                {
                    Success = true,
                    Message = "Event created successfully.",
                    Data = response
                });
        }


        //Controller to get event by Id
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<GetEventResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id,CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetEventByIdQuery(id), cancellationToken);

            if (result is null)
            {
                return NotFound(
                    new ApiResponse<GetEventResponse>
                    {
                        Success = false,
                        Message = "Event not found.",
                        Data = null
                    });
            }

            var response = mapper.Map<GetEventResponse>(result);

            return Ok(
                new ApiResponse<GetEventResponse>
                {
                    Success = true,
                    Message = "Event retrieved successfully.",
                    Data = response
                });
        }

        //Controller to update event
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
        {
            var response = mapper.Map<UpdateEventCommand>(request);
            await sender.Send(response, cancellationToken);

            return NoContent();
        }

        //Controller to publish event
        [HttpPost("{id:guid}/publish")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
        {
            // Send publish command
            await sender.Send(new PublishEventCommand(id), cancellationToken);

            // Return successful response
            return NoContent();
        }

        [HttpGet("my-events")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetMyEventsResponse>>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyEvents(CancellationToken cancellationToken)
        {
            // Get current authenticated user ID
            var userId = Guid.Parse(User.FindFirst("sub")!.Value);

            // Send query
            var result = await sender.Send(new GetMyEventsQuery(userId),cancellationToken);

            // Return events
            return Ok(new ApiResponse<IReadOnlyList<GetMyEventsResponse>>
            {
                Success = true,
                Message = "Events retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Cancel(Guid id,CancellationToken cancellationToken)
        {
            // Send cancel command
            await sender.Send(new CancelEventCommand(id),cancellationToken);

            return NoContent();
        }
    }
}
