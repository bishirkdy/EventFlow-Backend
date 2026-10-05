using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Application.Features.Attendance.Commands.CheckInManual;
using EventFlow.Operations.Application.Features.Attendance.Commands.CheckInQr;
using EventFlow.Operations.Application.Features.Attendance.Commands.CheckOut;
using EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceDashboard;
using EventFlow.Operations.Application.Features.Attendance.Queries.GetParticipantAttendanceHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Operations.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/operations/events/{eventId:guid}/attendance")]
public sealed class AttendanceController(ISender sender) : ControllerBase
{
    [HttpPost("check-in/qr")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Qr(
        Guid eventId,
        CheckInRequest request,
        [FromHeader(Name = "Authorization")] string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        var dto = await sender.Send(
            new CheckInQrCommand(
                eventId,
                request.QrCode,
                request.SectionId,
                request.SessionId,
                request.Method,
                bearerToken),
            cancellationToken);

        return Ok(ApiResponse<AttendanceDto>.Success(
            dto,
            "Participant checked in successfully."));
    }

    [HttpPost("check-in/manual")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Manual(
        Guid eventId,
        ManualCheckInRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await sender.Send(
            new CheckInManualCommand(
                eventId,
                request.RegistrationId,
                request.SectionId,
                request.SessionId),
            cancellationToken);

        return Ok(ApiResponse<AttendanceDto>.Success(
            dto,
            "Participant checked in manually."));
    }

    [HttpPost("{attendanceId:guid}/check-out")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Out(
        Guid eventId,
        Guid attendanceId,
        CancellationToken cancellationToken = default)
    {
        var dto = await sender.Send(
            new CheckOutCommand(eventId, attendanceId),
            cancellationToken);

        return Ok(ApiResponse<AttendanceDto>.Success(
            dto,
            "Participant checked out successfully."));
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Dash(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var dto = await sender.Send(
            new GetAttendanceDashboardQuery(eventId),
            cancellationToken);

        return Ok(ApiResponse<AttendanceDashboardDto>.Success(dto));
    }

    [HttpGet("history/{participantUserId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> History(
        Guid eventId,
        Guid participantUserId,
        CancellationToken cancellationToken = default)
    {
        var dto = await sender.Send(
            new GetParticipantAttendanceHistoryQuery(eventId, participantUserId),
            cancellationToken);

        return Ok(ApiResponse<AttendanceHistoryDto>.Success(dto));
    }
}
