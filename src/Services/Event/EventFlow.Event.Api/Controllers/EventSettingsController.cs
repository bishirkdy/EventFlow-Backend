using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.EventSettings;
using EventFlow.Event.Application.Features.EventSettings.Commands.ResetEventSettings;
using EventFlow.Event.Application.Features.EventSettings.Commands.UpdateEventSettings;
using EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/events-settings")]
    [Authorize]
    public class EventSettingsController(ISender sender , IMapper mapper) : ControllerBase
    {
        
        [HttpGet("{eventId:guid}/settings")]
        [ProducesResponseType(typeof(ApiResponse<GetEventSettingsResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSettings(Guid eventId,CancellationToken cancellationToken)
        {
            // Send query
            var result = await sender.Send(new GetEventSettingsQuery(eventId), cancellationToken);

            // Check whether settings exist
            if (result is null)
                return NotFound();

            // Return settings
            return Ok(new ApiResponse<GetEventSettingsResponse>
            {
                Success = true,
                Message = "Event settings retrieved successfully.",
                Data = result
            });
        }

        [HttpPut("{eventId:guid}/settings")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateSettings(Guid eventId,[FromBody] UpdateEventSettingsRequest request, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<UpdateEventSettingsCommand>(request) with
            {
                EventId = eventId
            };

            // Send command
            await sender.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{eventId:guid}/settings/reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetSettings(Guid eventId, CancellationToken cancellationToken)
        {
            // Send reset command
            await sender.Send(new ResetEventSettingsCommand(eventId), cancellationToken);
            return NoContent();
        }
    }
}
