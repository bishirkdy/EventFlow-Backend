using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Venues;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Venues.Commands.CreateVenue;
using EventFlow.Event.Application.Features.Venues.Commands.DeleteVenue;
using EventFlow.Event.Application.Features.Venues.Commands.UpdateVenue;
using EventFlow.Event.Application.Features.Venues.Commands.UpdateVenueCapacity;
using EventFlow.Event.Application.Features.Venues.Queries.GetVenueById;
using EventFlow.Event.Application.Features.Venues.Queries.GetVenuesByEvent;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
[Authorize]
public sealed class VenueController(ISender sender, IEventPreviewAccess previewAccess) : ControllerBase
{
    [HttpPost("{eventId:guid}/venues")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateVenue(Guid eventId, [FromForm] CreateVenueRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Image?.OpenReadStream();
        var image = request.Image is null ? null : new UploadedFile(
            stream!, request.Image.FileName, request.Image.ContentType, request.Image.Length);

        var command = new CreateVenueCommand(
            eventId, request.Name, request.Description, request.Address, request.Capacity, image);

        var venueId = await sender.Send(command, cancellationToken);
        return Ok(ApiResponse<Guid>.Success(venueId, "Venue created successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/venues")]
    public async Task<IActionResult> GetVenuesByEvent(Guid eventId, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
        {
            return Ok(ApiResponse<IReadOnlyList<GetVenuesByEventResponse>>.Success([], "Venues retrieved successfully."));
        }

        var venues = await sender.Send(new GetVenuesByEventQuery(eventId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<GetVenuesByEventResponse>>.Success(venues, "Venues retrieved successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/venues/{id:guid}")]
    public async Task<IActionResult> GetVenueById(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
            throw new NotFoundException("Venue not found.");

        var venue = await sender.Send(new GetVenueByIdQuery(id, eventId), cancellationToken);
        if (venue is null) throw new NotFoundException("Venue not found.");
        return Ok(ApiResponse<GetVenueByIdResponse>.Success(venue, "Venue retrieved successfully."));
    }

    [HttpPut("{eventId:guid}/venues/{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateVenue(Guid eventId, Guid id, [FromForm] UpdateVenueRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Image?.OpenReadStream();
        var image = request.Image is null ? null : new UploadedFile(
            stream!, request.Image.FileName, request.Image.ContentType, request.Image.Length);

        await sender.Send(new UpdateVenueCommand(
            id, eventId, request.Name, request.Description, request.Address, request.Capacity, image), cancellationToken);

        return Ok(ApiResponse<bool>.Success(true, "Venue updated successfully."));
    }

    [HttpDelete("{eventId:guid}/venues/{id:guid}")]
    public async Task<IActionResult> DeleteVenue(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteVenueCommand(id, eventId), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Venue deleted successfully."));
    }

    [HttpPatch("{eventId:guid}/venues/{id:guid}/capacity")]
    public async Task<IActionResult> UpdateVenueCapacity(Guid eventId, Guid id, int capacity, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateVenueCapacityCommand(id, eventId, capacity), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Venue capacity updated successfully."));
    }
}
