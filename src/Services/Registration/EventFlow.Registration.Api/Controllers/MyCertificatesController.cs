using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Certificates;
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
        var dto = await sender.Send(
            new GetMyCertificatesAcrossEventsQuery(),
            cancellationToken);

        return Ok(ApiResponse<List<CertificateDto>>.Success(
            dto,
            dto.Count == 0
                ? "No certificates have been issued for you yet."
                : "Certificates found."));
    }
}
