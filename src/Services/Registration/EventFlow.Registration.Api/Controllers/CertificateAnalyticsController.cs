using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateAnalytics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/certificates/analytics")]
[Authorize]
public sealed class CertificateAnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid eventId, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new GetCertificateAnalyticsQuery(eventId), cancellationToken);

        return Ok(ApiResponse<GetCertificateAnalyticsResponse>.Success(dto, "Certificate analytics computed."));
    }
}
