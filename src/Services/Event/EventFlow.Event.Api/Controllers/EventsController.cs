using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.Events;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Authentication;
using EventFlow.Event.Application.Abstractions.Storage;
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
    public sealed class EventsController(ISender sender,IMapper mapper, ICurrentUserService currentUserService) : ControllerBase
    {
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<CreateEventResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromForm] CreateEventRequest request, CancellationToken cancellationToken)
        {
            var images = request.Images
                .Select(file => new UploadedFile(
                    file.OpenReadStream(),
                    file.FileName,
                    file.ContentType,
                    file.Length))
                .ToList();

            try
            {
                var command = new CreateEventCommand(
                    request.Name,
                    request.Description,
                    request.EventType,
                    request.SubType,
                    request.StartDate,
                    request.EndDate,
                    request.TimeZone,
                    images);

                var result = await sender.Send(command, cancellationToken);
                var response = mapper.Map<CreateEventResponse>(result);

                return StatusCode(
                    StatusCodes.Status201Created,
                    new ApiResponse<CreateEventResponse>
                    {
                        Success = true,
                        Message = "Event created successfully.",
                        Data = response
                    });
            }
            finally
            {
                foreach (var image in images)
                {
                    await image.Content.DisposeAsync();
                }
            }
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<GetEventResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
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

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
        {
            var command = mapper.Map<UpdateEventCommand>(request) with
            {
                Id = id
            };

            await sender.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpPost("{id:guid}/publish")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
        {
            await sender.Send(new PublishEventCommand(id), cancellationToken);

            return NoContent();
        }

        [HttpGet("my-events")]
        [ProducesResponseType(
            typeof(ApiResponse<IReadOnlyList<GetMyEventsResponse>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyEvents(
            CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;

            var result = await sender.Send(
                new GetMyEventsQuery(userId),
                cancellationToken);

            return Ok(
                new ApiResponse<IReadOnlyList<GetMyEventsResponse>>
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
        public async Task<IActionResult> Cancel(
            Guid id,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new CancelEventCommand(id),
                cancellationToken);

            return NoContent();
        }
    }
}
