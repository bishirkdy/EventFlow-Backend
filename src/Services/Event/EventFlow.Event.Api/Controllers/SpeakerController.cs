using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Speakers;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Application.Features.Speakers.Commands.CreateSpeaker;
using EventFlow.Event.Application.Features.Speakers.Commands.DeleteSpeaker;
using EventFlow.Event.Application.Features.Speakers.Commands.UpdateSpeaker;
using EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakerById;
using EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakersByEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
[Authorize]
public sealed class SpeakerController(ISender sender) : ControllerBase
{
    [HttpPost("{eventId:guid}/speakers")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(Guid eventId, [FromForm] CreateSpeakerRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Image?.OpenReadStream();
        var image = request.Image is null ? null : new UploadedFile(stream!, request.Image.FileName, request.Image.ContentType, request.Image.Length);
        var id = await sender.Send(new CreateSpeakerCommand(eventId, request.Name, request.Bio, request.Designation, request.Organization, request.Email, request.DisplayOrder, image), cancellationToken);
        return Ok(new ApiResponse<Guid> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Speaker created successfully.", Data = id });
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/speakers")]
    public async Task<IActionResult> GetAll(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSpeakersByEventQuery(eventId), cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<GetSpeakersByEventResponse>> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Speakers retrieved successfully.", Data = result });
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/speakers/{id:guid}")]
    public async Task<IActionResult> GetById(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSpeakerByIdQuery(id, eventId), cancellationToken);
        if (result is null) throw new NotFoundException("Speaker not found.");
        return Ok(new ApiResponse<GetSpeakerByIdResponse> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Speaker retrieved successfully.", Data = result });
    }

    [HttpPut("{eventId:guid}/speakers/{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid eventId, Guid id, [FromForm] UpdateSpeakerRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Image?.OpenReadStream();
        var image = request.Image is null ? null : new UploadedFile(stream!, request.Image.FileName, request.Image.ContentType, request.Image.Length);
        await sender.Send(new UpdateSpeakerCommand(id, eventId, request.Name, request.Bio, request.Designation, request.Organization, request.Email, request.DisplayOrder, request.IsActive, image), cancellationToken);
        return Ok(new ApiResponse<object?> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Speaker updated successfully.", Data = null });
    }

    [HttpDelete("{eventId:guid}/speakers/{id:guid}")]
    public async Task<IActionResult> Delete(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteSpeakerCommand(id, eventId), cancellationToken);
        return Ok(new ApiResponse<object?> { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Message = "Speaker deleted successfully.", Data = null });
    }
}
