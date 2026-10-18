using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Features.Analytics.Queries.GetContentAnalytics;
using EventFlow.Event.Application.Features.Analytics.Queries.GetEventOverviewAnalytics;
using EventFlow.Event.Application.Features.Analytics.Queries.GetProgrammeAnalytics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        var result = await sender.Send(new GetEventOverviewAnalyticsQuery(eventId), cancellationToken);

        if (result is null)
        {
            return NotFound(ApiResponse<GetEventOverviewAnalyticsResponse>.Fail(
                ["Event not found."],
                "Event not found.",
                HttpStatusCode.NotFound));
        }

        return Ok(ApiResponse<GetEventOverviewAnalyticsResponse>.Success(
            result,
            "Event overview analytics retrieved successfully."));
    }

    [HttpGet("programme")]
    [ProducesResponseType(typeof(ApiResponse<GetProgrammeAnalyticsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Programme(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProgrammeAnalyticsQuery(eventId), cancellationToken);

        return Ok(ApiResponse<GetProgrammeAnalyticsResponse>.Success(
            result,
            "Programme analytics retrieved successfully."));
    }

    [HttpGet("content")]
    [ProducesResponseType(typeof(ApiResponse<GetContentAnalyticsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Content(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContentAnalyticsQuery(eventId), cancellationToken);

        return Ok(ApiResponse<GetContentAnalyticsResponse>.Success(
            result,
            "Content analytics retrieved successfully."));
    }
}
