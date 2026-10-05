using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.Request.OrganizerRequests;
using EventFlow.Identity.Application.Features.Commands.AssignOrganizer;
using EventFlow.Identity.Application.Features.Commands.RemoveOrganizer;
using EventFlow.Identity.Application.Features.Queries.GetEventTeam;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/events/{eventId:guid}/team")]
public sealed class EventTeamController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTeam(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var members = await sender.Send(
            new GetEventTeamQuery(eventId),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<EventTeamMemberResponse>>.Success(
            members,
            "Event team retrieved successfully."));
    }

    [HttpPost("organizers")]
    public async Task<IActionResult> AssignOrganizer(
        Guid eventId,
        [FromBody] AssignOrganizerRequest request,
        CancellationToken cancellationToken)
    {
        var roleId = await sender.Send(
            new AssignOrganizerCommand(eventId, request.Email),
            cancellationToken);

        return Ok(ApiResponse<Guid>.Success(
            roleId,
            "Organizer assigned successfully."));
    }

    [HttpDelete("organizers/{userId:guid}")]
    public async Task<IActionResult> RemoveOrganizer(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new RemoveOrganizerCommand(eventId, userId),
            cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Organizer removed successfully."));
    }
}
