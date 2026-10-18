using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Common;
using EventFlow.Identity.Api.Contracts.Request.EventInvitations;
using EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitation;
using EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

// Public endpoints for invitation acceptance
[ApiController]
[Route("api/v1/invitations")]
[AllowAnonymous]
public sealed class PublicInvitationController(ISender sender) : ControllerBase
{
    [HttpGet("{token}")]
    public async Task<IActionResult> GetInvitation(
        string token,
        CancellationToken cancellationToken)
    {
        var invitation = await sender.Send(
            new GetEventInvitationQuery(token),
            cancellationToken);

        return Ok(ApiResponse<GetEventInvitationResponse>.Success(
            invitation,
            "Invitation retrieved successfully."));
    }

    [HttpPost("{token}/accept")]
    public async Task<IActionResult> AcceptInvitation(
        string token,
        [FromBody] AcceptInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new AcceptEventInvitationCommand(
                token,
                request.Password,
                request.UserName,
                request.FirstName,
                request.LastName),
            cancellationToken);

        // A new account is signed in straight away so the invitation link can
        // send the invitee to their dashboard without a separate login.
        if (response.AccessToken is not null && response.RefreshToken is not null)
        {
            Response.SetAuthCookies(response.AccessToken, response.RefreshToken);
        }

        return Ok(ApiResponse<AcceptEventInvitationResponse>.Success(response, "Invitation accepted successfully."));
    }
}
