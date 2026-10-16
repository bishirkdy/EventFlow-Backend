using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitation;
using EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitationForExistingUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

// Authenticated endpoint used when an already signed-in user accepts an invitation.
[Authorize]
[ApiController]
[Route("api/v1/invitations")]
public sealed class EventInvitationAcceptanceController(ISender sender) : ControllerBase
{
    [HttpPost("{token}/accept-existing")]
    public async Task<IActionResult> AcceptForExistingUser(
        string token,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new AcceptEventInvitationForExistingUserCommand(token),
            cancellationToken);

        return Ok(ApiResponse<AcceptEventInvitationResponse>.Success(
            response,
            "Invitation accepted successfully."));
    }
}
