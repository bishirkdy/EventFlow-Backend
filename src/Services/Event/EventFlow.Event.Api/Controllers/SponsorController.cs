using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Sponsors;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Sponsors.Commands.CreateSponsor;
using EventFlow.Event.Application.Features.Sponsors.Commands.DeleteSponsor;
using EventFlow.Event.Application.Features.Sponsors.Commands.UpdateSponsor;
using EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorById;
using EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorsByEvent;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
[Authorize]
public sealed class SponsorController(ISender sender, IEventPreviewAccess previewAccess) : ControllerBase
{
    [HttpPost("{eventId:guid}/sponsors")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(Guid eventId, [FromForm] CreateSponsorRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Logo?.OpenReadStream();
        var logo = request.Logo is null ? null : new UploadedFile(stream!, request.Logo.FileName, request.Logo.ContentType, request.Logo.Length);
        var id = await sender.Send(new CreateSponsorCommand(eventId, request.Name, request.Description, request.WebsiteUrl, request.SponsorLevel, request.DisplayOrder, logo), cancellationToken);
        return Ok(ApiResponse<Guid>.Success(id, "Sponsor created successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/sponsors")]
    public async Task<IActionResult> GetAll(Guid eventId, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
        {
            return Ok(ApiResponse<IReadOnlyList<GetSponsorsByEventResponse>>.Success([], "Sponsors retrieved successfully."));
        }

        var result = await sender.Send(new GetSponsorsByEventQuery(eventId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<GetSponsorsByEventResponse>>.Success(result, "Sponsors retrieved successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/sponsors/{id:guid}")]
    public async Task<IActionResult> GetById(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
            throw new NotFoundException("Sponsor not found.");

        var result = await sender.Send(new GetSponsorByIdQuery(id, eventId), cancellationToken);
        if (result is null) throw new NotFoundException("Sponsor not found.");
        return Ok(ApiResponse<GetSponsorByIdResponse>.Success(result, "Sponsor retrieved successfully."));
    }

    [HttpPut("{eventId:guid}/sponsors/{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid eventId, Guid id, [FromForm] UpdateSponsorRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Logo?.OpenReadStream();
        var logo = request.Logo is null ? null : new UploadedFile(stream!, request.Logo.FileName, request.Logo.ContentType, request.Logo.Length);
        await sender.Send(new UpdateSponsorCommand(id, eventId, request.Name, request.Description, request.WebsiteUrl, request.SponsorLevel, request.DisplayOrder, request.IsActive, logo), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Sponsor updated successfully."));
    }

    [HttpDelete("{eventId:guid}/sponsors/{id:guid}")]
    public async Task<IActionResult> Delete(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteSponsorCommand(id, eventId), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Sponsor deleted successfully."));
    }
}
