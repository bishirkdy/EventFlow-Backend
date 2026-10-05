using System.Net;
using AutoMapper;
using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Events;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Events.Commands.CancelEvent;
using EventFlow.Event.Application.Features.Events.Commands.ClaimEventOwner;
using EventFlow.Event.Application.Features.Events.Commands.CreateEvent;
using EventFlow.Event.Application.Features.Events.Commands.PublishEvent;
using EventFlow.Event.Application.Features.Events.Commands.UpdateEvent;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using EventFlow.Event.Application.Features.Events.Queries.GetMyEvents;
using EventFlow.Event.Application.Features.Events.Queries.GetPublicEvents;
using EventFlow.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
[Authorize]
public sealed class EventsController(
    ISender sender,
    IMapper mapper,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpPost("create")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<CreateEventResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromForm] CreateEventRequest request,
        CancellationToken cancellationToken)
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
                request.EventTypeId,
                request.SubType,
                request.StartDate,
                request.EndDate,
                request.TimeZone,
                images);

            var result = await sender.Send(command, cancellationToken);
            var response = mapper.Map<CreateEventResponse>(result);

            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<CreateEventResponse>.Success(
                    response,
                    "Event created successfully.",
                    HttpStatusCode.Created));
        }
        finally
        {
            foreach (var image in images)
            {
                await image.Content.DisposeAsync();
            }
        }
    }

    [AllowAnonymous]
    [HttpGet("published/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublishedById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEventByIdQuery(id, PublicOnly: true),
            cancellationToken);

        if (result is null)
        {
            return NotFound(ApiResponse<GetEventResponse>.Fail(
                ["Published event not found."],
                "Published event not found.",
                HttpStatusCode.NotFound));
        }

        return Ok(ApiResponse<GetEventResponse>.Success(
            mapper.Map<GetEventResponse>(result),
            "Published event retrieved successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEventByIdQuery(id), cancellationToken);

        if (result is null)
        {
            return NotFound(ApiResponse<GetEventResponse>.Fail(
                ["Event not found."],
                "Event not found.",
                HttpStatusCode.NotFound));
        }

        return Ok(ApiResponse<GetEventResponse>.Success(
            mapper.Map<GetEventResponse>(result),
            "Event retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEventRequest request,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<UpdateEventCommand>(request) with
        {
            Id = id
        };

        await sender.Send(command, cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Event updated successfully."));
    }

    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Publish(
        Guid id,
        CancellationToken cancellationToken)
    {
        await sender.Send(new PublishEventCommand(id), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Event published successfully."));
    }

    [AllowAnonymous]
    [HttpGet("public")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetEventResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicEvents(
        [FromQuery] int take = 3,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPublicEventsQuery(take), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventResponse>>.Success(
            mapper.Map<IReadOnlyList<GetEventResponse>>(result),
            "Public events retrieved successfully."));
    }

    [HttpGet("my-events")]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyList<GetMyEventsResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyEvents(CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetMyEventsQuery(currentUserService.UserId),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetMyEventsResponse>>.Success(
            result,
            "Events retrieved successfully."));
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelEventCommand(id), cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, "Event cancelled successfully."));
    }

    [HttpPost("{id:guid}/claim-owner")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClaimOwner(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(
            new ClaimEventOwnerCommand(id, currentUserService.UserId),
            cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, "Event owner assigned successfully."));
    }
}
