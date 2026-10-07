using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Application.Features.Certificates.Queries.VerifyCertificate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/certificates")]
public sealed class CertificateVerifyController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{certificateNumber}/verify")]
    public async Task<IActionResult> Verify(string certificateNumber,CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new VerifyCertificateQuery(certificateNumber), cancellationToken);

        return Ok(ApiResponse<CertificateVerifyDto>.Success(dto, "Certificate verified."));
    }
}
