using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.Request.PhotographerInvitations;
using EventFlow.Identity.Application.Features.Commands.CreatePhotographerInvitation;
using EventFlow.Identity.Application.Features.Commands.RevokePhotographerInvitation;
using EventFlow.Identity.Application.Features.Queries.GetEventPhotographerInvitations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/events/{eventId:guid}/photographers")]
public sealed class PhotographerInvitationController(ISender sender) : ControllerBase
{
    [HttpPost("invite")]
    public async Task<IActionResult> InvitePhotographer(
        Guid eventId,
        [FromBody] InvitePhotographerRequest request,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new CreatePhotographerInvitationCommand(eventId, request.Email),
            cancellationToken);

        return Ok(ApiResponse<CreatePhotographerInvitationResponse>.Success(
            response,
            "Photographer invitation created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetPhotographerInvitations(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var invitations = await sender.Send(
            new GetEventPhotographerInvitationsQuery(eventId),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventPhotographerInvitationsResponse>>.Success(
            invitations,
            "Photographer invitations retrieved successfully."));
    }

    [HttpDelete("{invitationId:guid}")]
    public async Task<IActionResult> RevokeInvitation(
        Guid eventId,
        Guid invitationId,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new RevokePhotographerInvitationCommand(eventId, invitationId),
            cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Invitation revoked successfully."));
    }
}
