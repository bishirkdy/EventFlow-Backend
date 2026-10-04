using EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificatesAcrossEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/certificates")]
public sealed class MyCertificatesController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetMyCertificatesAcrossEventsQuery(),
            cancellationToken);

        return Ok(result);
    }
}
