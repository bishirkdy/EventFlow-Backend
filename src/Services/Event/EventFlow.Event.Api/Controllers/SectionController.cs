using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Sections;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Features.Sections.Commands.CreateSection;
using EventFlow.Event.Application.Features.Sections.Commands.DeleteSection;
using EventFlow.Event.Application.Features.Sections.Commands.UpdateSection;
using EventFlow.Event.Application.Features.Sections.Queries.GetSectionById;
using EventFlow.Event.Application.Features.Sections.Queries.GetSectionsByEvent;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/section")]
[Authorize]
public class SectionController(ISender sender, IEventPreviewAccess previewAccess) : ControllerBase
{
    [HttpPost("{eventId:guid}/sections")]
    public async Task<IActionResult> CreateSection(Guid eventId, CreateSectionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateSectionCommand(
            eventId,
            request.Name,
            request.Description,
            request.DisplayOrder
        );

        var sectionId = await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<Guid>.Success(
            sectionId,
            "Section created successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/sections")]
    public async Task<IActionResult> GetSectionsByEvent(Guid eventId, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
        {
            return Ok(ApiResponse<IReadOnlyList<GetSectionsByEventResponse>>.Success(
                [],
                "Sections retrieved successfully."));
        }

        var sections = await sender.Send(new GetSectionsByEventQuery(eventId), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetSectionsByEventResponse>>.Success(
            sections,
            "Sections retrieved successfully."));
    }

    [AllowAnonymous]
    [HttpGet("sections/{id:guid}")]
    public async Task<IActionResult> GetSectionById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var section = await sender.Send(
            new GetSectionByIdQuery(id),
            cancellationToken);

        if (section is null ||
            !await previewAccess.CanReadAsync(section.EventId, cancellationToken))
            throw new NotFoundException("Section not found.");

        return Ok(ApiResponse<GetSectionByIdResponse>.Success(
            section,
            "Section retrieved successfully."));
    }

    [HttpPut("{eventId:guid}/sections/{id:guid}")]
    public async Task<IActionResult> UpdateSection(Guid eventId, Guid id, UpdateSectionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSectionCommand(
            id,
            eventId,
            request.Name,
            request.Description,
            request.DisplayOrder
        );

        await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Section updated successfully."));
    }

    [HttpDelete("{eventId:guid}/sections/{id:guid}")]
    public async Task<IActionResult> DeleteSection(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteSectionCommand(id, eventId), cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Section deleted successfully."));
    }
}
