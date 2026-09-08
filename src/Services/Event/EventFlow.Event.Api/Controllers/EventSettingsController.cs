using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/events-settings")]
    [Authorize]
    public class EventSettingsController(ISender sender ) : ControllerBase
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
    }
}
