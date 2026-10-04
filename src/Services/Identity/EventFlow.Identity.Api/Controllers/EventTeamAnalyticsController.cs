using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Features.Queries.GetTeamAnalytics;
using EventFlow.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/events/{eventId:guid}/team/analytics")]
public sealed class EventTeamAnalyticsController(
    ISender sender,
    IPermissionService permissions,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetTeamAnalyticsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                eventId,
                "event.view",
                cancellationToken))
        {
            return Forbid();
        }

        var result = await sender.Send(
            new GetTeamAnalyticsQuery(eventId),
            cancellationToken);

        return Ok(ApiResponse<GetTeamAnalyticsResponse>.Success(
            result,
            "Team analytics retrieved successfully."));
    }
}
