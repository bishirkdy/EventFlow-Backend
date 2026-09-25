using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Venues;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Application.Features.Venues.Commands.CreateVenue;
using EventFlow.Event.Application.Features.Venues.Commands.DeleteVenue;
using EventFlow.Event.Application.Features.Venues.Commands.UpdateVenue;
using EventFlow.Event.Application.Features.Venues.Commands.UpdateVenueCapacity;
using EventFlow.Event.Application.Features.Venues.Queries.GetVenueById;
using EventFlow.Event.Application.Features.Venues.Queries.GetVenuesByEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    [Authorize]
    public sealed class VenueController(ISender sender) : ControllerBase
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
            return Ok(new ApiResponse<Guid> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Venue created successfully.", Data = venueId });
        }

        [AllowAnonymous]
        [HttpGet("{eventId:guid}/venues")]
        public async Task<IActionResult> GetVenuesByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            var venues = await sender.Send(new GetVenuesByEventQuery(eventId), cancellationToken);
            return Ok(new ApiResponse<IReadOnlyList<GetVenuesByEventResponse>> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Venues retrieved successfully.", Data = venues });
        }

        [AllowAnonymous]
        [HttpGet("{eventId:guid}/venues/{id:guid}")]
        public async Task<IActionResult> GetVenueById(Guid eventId, Guid id, CancellationToken cancellationToken)
        {
            var venue = await sender.Send(new GetVenueByIdQuery(id, eventId), cancellationToken);
            if (venue is null) throw new NotFoundException("Venue not found.");
            return Ok(new ApiResponse<GetVenueByIdResponse> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Venue retrieved successfully.", Data = venue });
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

            return Ok(new ApiResponse<bool> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Venue updated successfully.", Data = true });
        }

        [HttpDelete("{eventId:guid}/venues/{id:guid}")]
        public async Task<IActionResult> DeleteVenue(Guid eventId, Guid id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteVenueCommand(id, eventId), cancellationToken);
            return Ok(new ApiResponse<object?> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Venue deleted successfully.", Data = null });
        }

        [HttpPatch("{eventId:guid}/venues/{id:guid}/capacity")]
        public async Task<IActionResult> UpdateVenueCapacity(Guid eventId, Guid id, int capacity, CancellationToken cancellationToken)
        {
            await sender.Send(new UpdateVenueCapacityCommand(id, eventId, capacity), cancellationToken);
            return Ok(new ApiResponse<object?> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Venue capacity updated successfully.", Data = null });
        }
    }
}
