using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.Sponsors;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Application.Features.Sponsors.Commands.CreateSponsor;
using EventFlow.Event.Application.Features.Sponsors.Commands.DeleteSponsor;
using EventFlow.Event.Application.Features.Sponsors.Commands.UpdateSponsor;
using EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorById;
using EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorsByEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
[Authorize]
public sealed class SponsorController(ISender sender) : ControllerBase
{
    [HttpPost("{eventId:guid}/sponsors")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(Guid eventId, [FromForm] CreateSponsorRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Logo?.OpenReadStream();
        var logo = request.Logo is null ? null : new UploadedFile(stream!, request.Logo.FileName, request.Logo.ContentType, request.Logo.Length);
        var id = await sender.Send(new CreateSponsorCommand(eventId, request.Name, request.Description, request.WebsiteUrl, request.SponsorLevel, request.DisplayOrder, logo), cancellationToken);
        return Ok(new ApiResponse<Guid> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Sponsor created successfully.", Data = id });
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/sponsors")]
    public async Task<IActionResult> GetAll(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSponsorsByEventQuery(eventId), cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<GetSponsorsByEventResponse>> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Sponsors retrieved successfully.", Data = result });
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/sponsors/{id:guid}")]
    public async Task<IActionResult> GetById(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSponsorByIdQuery(id, eventId), cancellationToken);
        if (result is null) throw new NotFoundException("Sponsor not found.");
        return Ok(new ApiResponse<GetSponsorByIdResponse> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Sponsor retrieved successfully.", Data = result });
    }

    [HttpPut("{eventId:guid}/sponsors/{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid eventId, Guid id, [FromForm] UpdateSponsorRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Logo?.OpenReadStream();
        var logo = request.Logo is null ? null : new UploadedFile(stream!, request.Logo.FileName, request.Logo.ContentType, request.Logo.Length);
        await sender.Send(new UpdateSponsorCommand(id, eventId, request.Name, request.Description, request.WebsiteUrl, request.SponsorLevel, request.DisplayOrder, request.IsActive, logo), cancellationToken);
        return Ok(new ApiResponse<object?> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Sponsor updated successfully.", Data = null });
    }

    [HttpDelete("{eventId:guid}/sponsors/{id:guid}")]
    public async Task<IActionResult> Delete(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteSponsorCommand(id, eventId), cancellationToken);
        return Ok(new ApiResponse<object?> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Sponsor deleted successfully.", Data = null });
    }
}
