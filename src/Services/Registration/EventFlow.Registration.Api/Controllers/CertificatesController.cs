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
    public async Task<IActionResult> GetSettings(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCertificateSettingsQuery(eventId), cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings(
        Guid eventId,
        UpsertCertificateSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpsertCertificateSettingsCommand(eventId, request), cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("eligibility")]
    public async Task<IActionResult> GetEligibility(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCertificateEligibilityQuery(eventId), cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        Guid eventId,
        GenerateCertificatesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GenerateCertificatesCommand(eventId, request), cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListCertificatesQuery(eventId), cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMine(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetMyCertificatesQuery(eventId), cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("{certificateId:guid}/revoke")]
    public async Task<IActionResult> Revoke(
        Guid eventId,
        Guid certificateId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RevokeCertificateCommand(eventId, certificateId), cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("{certificateId:guid}/download")]
    public async Task<IActionResult> Download(
        Guid eventId,
        Guid certificateId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DownloadCertificateQuery(eventId, certificateId), cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return BadRequest(result);
        }

        return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
    }
}
