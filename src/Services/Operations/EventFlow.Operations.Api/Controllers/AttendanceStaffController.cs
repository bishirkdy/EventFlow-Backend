using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Application.Features.AttendanceStaff.Commands.AssignAttendanceStaff;
using EventFlow.Operations.Application.Features.AttendanceStaff.Commands.RevokeAttendanceStaff;
using EventFlow.Operations.Application.Features.AttendanceStaff.Queries.GetAttendanceStaff;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Operations.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/operations/events/{eventId:guid}/attendance-staff")]
public sealed class AttendanceStaffController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AttendanceStaffDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        Guid eventId, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetAttendanceStaffQuery(eventId), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<AttendanceStaffDto>>.Success(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AttendanceStaffDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AttendanceStaffDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Assign(Guid eventId, AssignAttendanceStaffRequest request, CancellationToken cancellationToken = default)
    {
        var dto = await sender.Send(
            new AssignAttendanceStaffCommand(eventId,request.Email,request.ScopeType,request.ScopeId),cancellationToken);

        return Ok(ApiResponse<AttendanceStaffDto>.Success(
            dto,"Attendance staff assigned successfully."));
    }

    [HttpDelete("{assignmentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Revoke(Guid eventId, Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        await sender.Send(
            new RevokeAttendanceStaffCommand(eventId, assignmentId), cancellationToken);

        return Ok(ApiResponse<object?>.Success(null, "Attendance staff assignment revoked."));
    }
}
