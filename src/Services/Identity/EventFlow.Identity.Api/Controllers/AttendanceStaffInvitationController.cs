using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.Request.EventInvitations;
using EventFlow.Identity.Application.Features.EventInvitations.Commands.CreateEventInvitation;
using EventFlow.Identity.Application.Features.EventInvitations.Commands.RevokeEventInvitation;
using EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitations;
using EventFlow.Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/events/{eventId:guid}/attendance-staff")]
public sealed class AttendanceStaffInvitationController(ISender sender) : ControllerBase
{
    [HttpPost("invite")]
    public async Task<IActionResult> InviteAttendanceStaff(Guid eventId, [FromBody] InviteAttendanceStaffRequest request,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new CreateEventInvitationCommand(
                eventId,
                request.Email,
                EventInvitationType.AttendanceStaff,
                request.ScopeType,
                request.ScopeId),
            cancellationToken);

        return Ok(ApiResponse<CreateEventInvitationResponse>.Success(
            response, "Attendance staff invitation created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAttendanceStaffInvitations(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var invitations = await sender.Send(
            new GetEventInvitationsQuery(eventId, EventInvitationType.AttendanceStaff),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventInvitationsResponse>>.Success(
            invitations,
            "Attendance staff invitations retrieved successfully."));
    }

    [HttpDelete("{invitationId:guid}")]
    public async Task<IActionResult> RevokeInvitation(
        Guid eventId,
        Guid invitationId,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new RevokeEventInvitationCommand(eventId, invitationId),
            cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Invitation revoked successfully."));
    }
}
