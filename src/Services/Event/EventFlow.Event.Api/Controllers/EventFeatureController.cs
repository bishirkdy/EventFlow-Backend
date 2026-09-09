using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Application.Features.EventFeature.Commands.DisableEventFeature;
using EventFlow.Event.Application.Features.EventFeature.Commands.EnableEventFeature;
using EventFlow.Event.Application.Features.EventFeature.Commands.ResetEventFeatures;
using EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    [Authorize]
    public sealed class EventFeatureController(ISender sender) : ControllerBase
    {
        //Enable event features
        [HttpPost("{eventId:guid}/features/{featureId:guid}/enable")]
        public async Task<IActionResult> EnableFeature(Guid eventId,Guid featureId,CancellationToken cancellationToken)
        {
            await sender.Send(new EnableEventFeatureCommand(eventId, featureId),cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Feature enabled successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Disable event features
        [HttpPost("{eventId:guid}/features/{featureId:guid}/disable")]
        public async Task<IActionResult> DisableFeature(Guid eventId,Guid featureId,CancellationToken cancellationToken)
        {
            await sender.Send(new DisableEventFeatureCommand(eventId, featureId),cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Feature disabled successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Get Event features using event id
        [HttpGet("{eventId:guid}/features")]
        public async Task<IActionResult> GetEventFeatures(Guid eventId,CancellationToken cancellationToken)
        {
            var features = await sender.Send(new GetEventFeaturesQuery(eventId),cancellationToken);

            var response =new ApiResponse<IReadOnlyList<GetEventFeaturesResponse>>
                {
                    Success = true,
                    Message = "Event features retrieved successfully.",
                    Data = features
                };

            return Ok(response);
        }

        //Reset all features of a event
        [HttpPost("{eventId:guid}/features/reset")]
        public async Task<IActionResult> ResetEventFeatures(Guid eventId,CancellationToken cancellationToken)
        {
            // Send command
            await sender.Send(new ResetEventFeaturesCommand(eventId), cancellationToken);

            // Create API response
            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Event features reset successfully.",
                Data = null
            };

            return Ok(response);
        }

    }
}
