using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.Venues;
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
    public sealed class VenueController(ISender sender, IMapper mapper) : ControllerBase
    {
        [HttpPost("{eventId:guid}/venues")]
        public async Task<IActionResult> CreateVenue(Guid eventId, CreateVenueRequest request, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<CreateVenueCommand>(request) with
            {
                EventId = eventId
            };

            // Send command
            var venueId = await sender.Send(command,cancellationToken);

            // Create API response
            var response = new ApiResponse<Guid>
            {
                Success = true,
                Message = "Venue created successfully.",
                Data = venueId
            };

            return Ok(response);
        }

        //Get venues of events
        [HttpGet("{eventId:guid}/venues")]
        public async Task<IActionResult> GetVenuesByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            // Send query
            var venues = await sender.Send(new GetVenuesByEventQuery(eventId), cancellationToken);

            var response = new ApiResponse<IReadOnlyList<GetVenuesByEventResponse>>
            {
                Success = true,
                Message = "Venues retrieved successfully.",
                Data = venues
            };

            return Ok(response);
        }

        //Controller for get venue by id and event id
        [HttpGet("{eventId:guid}/venues/{id:guid}")]
        public async Task<IActionResult> GetVenueById(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            // Send query
            var venue = await sender.Send(new GetVenueByIdQuery(id, eventId) ,cancellationToken);

            if (venue is null)
                throw new NotFoundException("Venue not found.");

            var response = new ApiResponse<GetVenueByIdResponse>
            {
                Success = true,
                Message = "Venue retrieved successfully.",
                Data = venue
            };

            return Ok(response);
        }

        //Update Venue of event
        [HttpPut("{eventId:guid}/venues/{id:guid}")]
        public async Task<IActionResult> UpdateVenue(Guid eventId, Guid id, UpdateVenueRequest request, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<UpdateVenueCommand>(request) with
            {
                Id = id,
                EventId = eventId
            };

            // Send command
            await sender.Send(command, cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Venue updated successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Delete venue
        [HttpDelete("{eventId:guid}/venues/{id:guid}")]
        public async Task<IActionResult> DeleteVenue(Guid eventId, Guid id, CancellationToken cancellationToken)
        {
            // Send command
            await sender.Send(new DeleteVenueCommand(id, eventId), cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Venue deleted successfully.",
                Data = null
            };

            return Ok(response);
        }

        [HttpPatch("{eventId:guid}/venues/{id:guid}/capacity")]
        public async Task<IActionResult> UpdateVenueCapacity(Guid eventId, Guid id, int capacity, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = new UpdateVenueCapacityCommand(id,eventId, capacity);

            // Send command
            await sender.Send(command, cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Venue capacity updated successfully.",
                Data = null
            };

            return Ok(response);
        }
    }
}
