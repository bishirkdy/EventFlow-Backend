using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Application.Features.Certificates.Commands.GenerateCertificates;
using EventFlow.Registration.Application.Features.Certificates.Commands.RevokeCertificate;
using EventFlow.Registration.Application.Features.Certificates.Commands.UpsertCertificateSettings;
using EventFlow.Registration.Application.Features.Certificates.Queries.DownloadCertificate;
using EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateEligibility;
using EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateSettings;
using EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificates;
using EventFlow.Registration.Application.Features.Certificates.Queries.ListCertificates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/certificates")]
public sealed class CertificatesController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(Guid eventId, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new GetCertificateSettingsQuery(eventId), cancellationToken);

        return Ok(ApiResponse<CertificateSettingsDto>.Success(
            dto,
            "Certificate settings retrieved."));
    }

    [Authorize]
    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings(
        Guid eventId,
        UpsertCertificateSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new UpsertCertificateSettingsCommand(eventId, request), cancellationToken);

        return Ok(ApiResponse<CertificateSettingsDto>.Success(dto, "Certificate settings saved."));
    }

    [Authorize]
    [HttpGet("eligibility")]
    public async Task<IActionResult> GetEligibility(Guid eventId, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new GetCertificateEligibilityQuery(eventId), cancellationToken);

        return Ok(ApiResponse<CertificateEligibilityDto>.Success(dto,"Eligibility computed."));
    }

    [Authorize]
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(Guid eventId, GenerateCertificatesRequest request, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new GenerateCertificatesCommand(eventId, request), cancellationToken);

        var message = dto.Failed > 0
            ? $"Generated {dto.Generated} certificate(s), {dto.Skipped} skipped, {dto.Failed} failed."
            : $"Generated {dto.Generated} certificate(s), {dto.Skipped} skipped.";

        return Ok(ApiResponse<CertificateGenerationResultDto>.Success(
            dto,
            message));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List(Guid eventId,CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new ListCertificatesQuery(eventId), cancellationToken);

        return Ok(ApiResponse<List<CertificateDto>>.Success(dto, "Certificates loaded."));
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMine(Guid eventId, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new GetMyCertificatesQuery(eventId), cancellationToken);

        return Ok(ApiResponse<List<CertificateDto>>.Success(dto, dto.Count == 0
                ? "No certificate has been issued for you yet.": "Certificate found."));
    }

    [Authorize]
    [HttpPost("{certificateId:guid}/revoke")]
    public async Task<IActionResult> Revoke(
        Guid eventId,
        Guid certificateId,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new RevokeCertificateCommand(eventId, certificateId), cancellationToken);

        return Ok(ApiResponse<CertificateDto>.Success(
            dto,
            "Certificate revoked."));
    }

    [Authorize]
    [HttpGet("{certificateId:guid}/download")]
    public async Task<IActionResult> Download(Guid eventId, Guid certificateId,CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new DownloadCertificateQuery(eventId, certificateId), cancellationToken);

        return File(dto.Content, dto.ContentType, dto.FileName);
    }
}
