using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Speakers;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Features.Speakers.Commands.CreateSpeaker;
using EventFlow.Event.Application.Features.Speakers.Commands.DeleteSpeaker;
using EventFlow.Event.Application.Features.Speakers.Commands.UpdateSpeaker;
using EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakerById;
using EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakersByEvent;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
[Authorize]
public sealed class SpeakerController(ISender sender, IEventPreviewAccess previewAccess) : ControllerBase
{
    [HttpPost("{eventId:guid}/speakers")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(Guid eventId, [FromForm] CreateSpeakerRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Image?.OpenReadStream();
        var image = request.Image is null ? null : new UploadedFile(stream!, request.Image.FileName, request.Image.ContentType, request.Image.Length);
        var id = await sender.Send(new CreateSpeakerCommand(eventId, request.Name, request.Bio, request.Designation, request.Organization, request.Email, request.DisplayOrder, image), cancellationToken);
        return Ok(ApiResponse<Guid>.Success(id, "Speaker created successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/speakers")]
    public async Task<IActionResult> GetAll(Guid eventId, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
        {
            return Ok(ApiResponse<IReadOnlyList<GetSpeakersByEventResponse>>.Success([], "Speakers retrieved successfully."));
        }

        var result = await sender.Send(new GetSpeakersByEventQuery(eventId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<GetSpeakersByEventResponse>>.Success(result, "Speakers retrieved successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/speakers/{id:guid}")]
    public async Task<IActionResult> GetById(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
            throw new NotFoundException("Speaker not found.");

        var result = await sender.Send(new GetSpeakerByIdQuery(id, eventId), cancellationToken);
        if (result is null) throw new NotFoundException("Speaker not found.");
        return Ok(ApiResponse<GetSpeakerByIdResponse>.Success(result, "Speaker retrieved successfully."));
    }

    [HttpPut("{eventId:guid}/speakers/{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid eventId, Guid id, [FromForm] UpdateSpeakerRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Image?.OpenReadStream();
        var image = request.Image is null ? null : new UploadedFile(stream!, request.Image.FileName, request.Image.ContentType, request.Image.Length);
        await sender.Send(new UpdateSpeakerCommand(id, eventId, request.Name, request.Bio, request.Designation, request.Organization, request.Email, request.DisplayOrder, request.IsActive, image), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Speaker updated successfully."));
    }

    [HttpDelete("{eventId:guid}/speakers/{id:guid}")]
    public async Task<IActionResult> Delete(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteSpeakerCommand(id, eventId), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Speaker deleted successfully."));
    }
}
