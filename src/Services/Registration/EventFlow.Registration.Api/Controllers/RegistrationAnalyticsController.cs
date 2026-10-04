using EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/registrations/analytics")]
[Authorize]
public sealed class RegistrationAnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        Guid eventId,
        [FromQuery] int days = 30,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetRegistrationAnalyticsQuery(eventId, days),
            cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
