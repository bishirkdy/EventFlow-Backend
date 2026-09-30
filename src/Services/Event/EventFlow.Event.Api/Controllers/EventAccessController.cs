using EventFlow.Event.Application.Abstractions.Authorization;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventAuthorizationService = EventFlow.Event.Application.Abstractions.Authorization.IAuthorizationService;
namespace EventFlow.Event.Api.Controllers;

[ApiController]
[InternalServiceOnly]
[Route("api/v1/events/{eventId:guid}")]
public sealed class EventAccessController(
    IEventFeatureRepository eventFeatures,
    IFeatureRepository features,
    EventAuthorizationService permissions) : ControllerBase
{
    [HttpGet("features/registration")]
    public async Task<IActionResult> RegistrationFeature(Guid eventId, CancellationToken cancellationToken)
    {
        var feature = await features.GetByCodeAsync("registration", cancellationToken);
        if (feature is null)
        {
            return Ok(new { enabled = false });
        }

        var eventFeature = await eventFeatures.GetByEventAndFeatureAsync(eventId, feature.Id, cancellationToken);
        return Ok(new { enabled = eventFeature?.IsEnabled == true });
    }

    [HttpGet("registration-access")]
    public async Task<IActionResult> RegistrationAccess(
        Guid eventId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var allowed = await permissions.HasPermissionAsync(
            userId,
            eventId,
            "event.update",
            cancellationToken);

        return Ok(new { allowed });
    }
}
