using EventFlow.Event.Application.Features.EventFeature.Queries.GetRegistrationAccess;
using EventFlow.Event.Application.Features.EventFeature.Queries.GetRegistrationFeature;
using EventFlow.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[InternalServiceOnly]
[Route("api/v1/events/{eventId:guid}")]
public sealed class EventAccessController(ISender sender) : ControllerBase
{
    [HttpGet("features/registration")]
    public async Task<IActionResult> RegistrationFeature(Guid eventId, CancellationToken cancellationToken)
    {
        var enabled = await sender.Send(new GetRegistrationFeatureQuery(eventId), cancellationToken);

        return Ok(new { enabled });
    }

    [HttpGet("registration-access")]
    public async Task<IActionResult> RegistrationAccess(Guid eventId,[FromQuery] Guid userId,CancellationToken cancellationToken)
    {
        var allowed = await sender.Send(
            new GetRegistrationAccessQuery(eventId, userId),
            cancellationToken);

        return Ok(new { allowed });
    }
}
