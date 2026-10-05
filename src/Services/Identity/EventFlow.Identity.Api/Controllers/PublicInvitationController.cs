using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.Request.PhotographerInvitations;
using EventFlow.Identity.Application.Features.Commands.AcceptPhotographerInvitation;
using EventFlow.Identity.Application.Features.Queries.GetPhotographerInvitation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

// Public endpoints for invitation acceptance (no auth required).
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
            new GetPhotographerInvitationQuery(token),
            cancellationToken);

        return Ok(ApiResponse<GetPhotographerInvitationResponse>.Success(
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
            new AcceptPhotographerInvitationCommand(
                token,
                request.Password,
                request.UserName,
                request.FirstName,
                request.LastName),
            cancellationToken);

        return Ok(ApiResponse<AcceptPhotographerInvitationResponse>.Success(
            response,
            "Invitation accepted successfully."));
    }
}
