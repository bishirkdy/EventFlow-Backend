using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Features.Queries.GetTeamAnalytics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/events/{eventId:guid}/team/analytics")]
public sealed class EventTeamAnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetTeamAnalyticsQuery(eventId),
            cancellationToken);

        return Ok(ApiResponse<GetTeamAnalyticsResponse>.Success(
            result,
            "Team analytics retrieved successfully."));
    }
}
