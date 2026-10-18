using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Features.SessionSpeakers.Commands.AssignSpeaker;
using EventFlow.Event.Application.Features.SessionSpeakers.Commands.UnassignSpeaker;
using EventFlow.Event.Application.Features.SessionSpeakers.Queries.GetSessionSpeakers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/sessions/{sessionId:guid}/speakers")]
[Authorize]
public sealed class SessionSpeakerController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get(Guid eventId, Guid sessionId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSessionSpeakersQuery(eventId, sessionId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<GetSessionSpeakersResponse>>.Success(result, "Session speakers retrieved successfully."));
    }

    [HttpPost("{speakerId:guid}")]
    public async Task<IActionResult> Assign(Guid eventId, Guid sessionId, Guid speakerId, CancellationToken cancellationToken)
    {
        await sender.Send(new AssignSpeakerCommand(eventId, sessionId, speakerId), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Speaker assigned successfully."));
    }

    [HttpDelete("{speakerId:guid}")]
    public async Task<IActionResult> Unassign(Guid eventId, Guid sessionId, Guid speakerId, CancellationToken cancellationToken)
    {
        await sender.Send(new UnassignSpeakerCommand(eventId, sessionId, speakerId), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Speaker removed from session successfully."));
    }
}
