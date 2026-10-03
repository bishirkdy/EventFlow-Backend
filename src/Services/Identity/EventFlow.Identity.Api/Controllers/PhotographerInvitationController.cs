using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Features.Commands.CreatePhotographerInvitation;
using EventFlow.Identity.Application.Features.Commands.AcceptPhotographerInvitation;
using EventFlow.Identity.Application.Features.Commands.RevokePhotographerInvitation;
using EventFlow.Identity.Application.Features.Queries.GetPhotographerInvitation;
using EventFlow.Identity.Application.Features.Queries.GetEventPhotographerInvitations;
using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/photographers")]
public sealed class PhotographerInvitationController(
    IMediator mediator,
    IPermissionService permissions,
    ICurrentUserService currentUser) : ControllerBase
{
    // POST /api/v1/events/{eventId}/photographers/invite
    [Authorize]
    [HttpPost("invite")]
    public async Task<IActionResult> InvitePhotographer(
        Guid eventId,
        [FromBody] InvitePhotographerRequest request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                eventId,
                "event.team.manage",
                cancellationToken))
        {
            return Forbid();
        }

        // Get Photographer role ID
        var photographerRole = await GetPhotographerRoleId(cancellationToken);
        if (photographerRole == Guid.Empty)
        {
            return Conflict(ApiResponse<object?>.Fail(
                ["The Photographer role is not configured."],
                "Photographer role is unavailable."));
        }

        var command = new CreatePhotographerInvitationCommand(
            eventId,
            request.Email.Trim(),
            currentUser.UserId,
            photographerRole);

        var response = await mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<CreatePhotographerInvitationResponse>.Success(
            response,
            "Photographer invitation created successfully."));
    }

    // GET /api/v1/events/{eventId}/photographers
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetPhotographerInvitations(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                eventId,
                "event.team.manage",
                cancellationToken))
        {
            return Forbid();
        }

        var query = new GetEventPhotographerInvitationsQuery(eventId);
        var invitations = await mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventPhotographerInvitationsResponse>>.Success(
            invitations,
            "Photographer invitations retrieved successfully."));
    }

    // DELETE /api/v1/events/{eventId}/photographers/{invitationId}
    [Authorize]
    [HttpDelete("{invitationId:guid}")]
    public async Task<IActionResult> RevokeInvitation(
        Guid eventId,
        Guid invitationId,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                eventId,
                "event.team.manage",
                cancellationToken))
        {
            return Forbid();
        }

        var command = new RevokePhotographerInvitationCommand(invitationId, currentUser.UserId);
        await mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Invitation revoked successfully."));
    }

    private async Task<Guid> GetPhotographerRoleId(CancellationToken cancellationToken)
    {
        // This would ideally be cached or injected
        // For now, using the fixed seed ID
        return Guid.Parse("10000000-0000-0000-0000-000000000006");
    }

    public sealed record InvitePhotographerRequest(string Email);
}

// Public endpoints for invitation acceptance (no auth required)
[ApiController]
[Route("api/v1/invitations")]
[AllowAnonymous]
public sealed class PublicInvitationController(IMediator mediator) : ControllerBase
{
    // GET /api/v1/invitations/{token}
    [HttpGet("{token}")]
    public async Task<IActionResult> GetInvitation(
        string token,
        CancellationToken cancellationToken)
    {
        var query = new GetPhotographerInvitationQuery(token);
        var invitation = await mediator.Send(query, cancellationToken);

        if (invitation is null)
        {
            return NotFound(ApiResponse<object?>.Fail(
                ["Invitation not found."],
                "Invalid invitation token."));
        }

        return Ok(ApiResponse<GetPhotographerInvitationResponse>.Success(
            invitation,
            "Invitation retrieved successfully."));
    }

    // POST /api/v1/invitations/{token}/accept
    [HttpPost("{token}/accept")]
    public async Task<IActionResult> AcceptInvitation(
        string token,
        [FromBody] AcceptInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AcceptPhotographerInvitationCommand(
            token,
            request.Password,
            request.UserName,
            request.FirstName,
            request.LastName);

        var response = await mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<AcceptPhotographerInvitationResponse>.Success(
            response,
            "Invitation accepted successfully."));
    }

    public sealed record AcceptInvitationRequest(
        string Password,
        string UserName,
        string FirstName,
        string LastName);
}