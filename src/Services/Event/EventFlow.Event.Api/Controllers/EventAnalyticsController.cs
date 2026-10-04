using EventFlow.Event.Application.Features.Analytics.Queries.GetContentAnalytics;
using EventFlow.Event.Application.Features.Analytics.Queries.GetEventOverviewAnalytics;
using EventFlow.Event.Application.Features.Analytics.Queries.GetProgrammeAnalytics;
using EventFlow.Contracts.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/analytics")]
[Authorize]
public sealed class EventAnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<GetEventOverviewAnalyticsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Overview(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEventOverviewAnalyticsQuery(eventId),
            cancellationToken);

        if (result is null)
        {
            return NotFound(new ApiResponse<GetEventOverviewAnalyticsResponse>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Event not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<GetEventOverviewAnalyticsResponse>
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Event overview analytics retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("programme")]
    [ProducesResponseType(typeof(ApiResponse<GetProgrammeAnalyticsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Programme(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProgrammeAnalyticsQuery(eventId),
            cancellationToken);

        return Ok(new ApiResponse<GetProgrammeAnalyticsResponse>
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Programme analytics retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("content")]
    [ProducesResponseType(typeof(ApiResponse<GetContentAnalyticsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Content(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetContentAnalyticsQuery(eventId),
            cancellationToken);

        return Ok(new ApiResponse<GetContentAnalyticsResponse>
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Content analytics retrieved successfully.",
            Data = result
        });
    }
}
