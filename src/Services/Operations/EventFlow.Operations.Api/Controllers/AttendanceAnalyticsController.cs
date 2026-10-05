using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceAnalytics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Operations.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/operations/events/{eventId:guid}/attendance/analytics")]
public sealed class AttendanceAnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetAttendanceAnalyticsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get(
        Guid eventId,
        [FromQuery] int days = 30,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetAttendanceAnalyticsQuery(eventId, days),
            cancellationToken);

        return Ok(ApiResponse<GetAttendanceAnalyticsResponse>.Success(
            result,
            "Attendance analytics computed."));
    }
}
