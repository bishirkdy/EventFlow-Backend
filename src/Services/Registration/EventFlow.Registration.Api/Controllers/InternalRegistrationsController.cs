using EventFlow.Registration.Application.Features.Registrations.Queries.GetRegisteredEventIds;
using EventFlow.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/registrations/internal")]
public sealed class InternalRegistrationsController(ISender sender) : ControllerBase
{
    [InternalServiceOnly]
    [HttpGet("users/{userId:guid}/event-ids")]
    public async Task<IActionResult> GetRegisteredEventIds(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetRegisteredEventIdsQuery(userId),
            cancellationToken);

        return Ok(result);
    }
}
