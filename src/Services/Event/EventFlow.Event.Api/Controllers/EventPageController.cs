using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.EventPages;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Features.EventPages.Commands.CreateEventPage;
using EventFlow.Event.Application.Features.EventPages.Commands.DeleteEventPage;
using EventFlow.Event.Application.Features.EventPages.Commands.EnsureEventWebsite;
using EventFlow.Event.Application.Features.EventPages.Commands.PublishEventPage;
using EventFlow.Event.Application.Features.EventPages.Commands.ReorderEventPages;
using EventFlow.Event.Application.Features.EventPages.Commands.UnpublishEventPage;
using EventFlow.Event.Application.Features.EventPages.Commands.UpdateEventPage;
using EventFlow.Event.Application.Features.EventPages.Queries.GetEventPageById;
using EventFlow.Event.Application.Features.EventPages.Queries.GetEventPagesByEvent;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/event-page")]
[Authorize]
public class EventPageController(ISender sender, IEventPreviewAccess previewAccess) : ControllerBase
{
    [HttpPost("{eventId:guid}/pages")]
    public async Task<IActionResult> CreateEventPage(Guid eventId, CreateEventPageRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateEventPageCommand(
            eventId,
            request.Name,
            request.Slug,
            request.PageType
        );

        var pageId = await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<Guid>.Success(
            pageId,
            "Event page created successfully."));
    }

    [HttpPost("{eventId:guid}/ensure-website")]
    public async Task<IActionResult> EnsureEventWebsite(Guid eventId, CancellationToken cancellationToken)
    {
        await sender.Send(new EnsureEventWebsiteCommand(eventId), cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Event website pages are ready."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/pages")]
    public async Task<IActionResult> GetEventPagesByEvent(Guid eventId, CancellationToken cancellationToken)
    {
        var pages = await sender.Send(new GetEventPagesByEventQuery(eventId, IncludeUnpublished: false), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventPagesByEventResponse>>.Success(
            pages,
            "Event pages retrieved successfully."));
    }

    [HttpGet("{eventId:guid}/manage-pages")]
    public async Task<IActionResult> GetManageEventPages(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        await EnsureCanPreviewAsync(eventId, cancellationToken);

        var pages = await sender.Send(
            new GetEventPagesByEventQuery(eventId, IncludeUnpublished: true),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventPagesByEventResponse>>.Success(
            pages,
            "Event pages retrieved successfully."));
    }

    [HttpGet("{eventId:guid}/preview")]
    public async Task<IActionResult> GetPreviewPages(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        await EnsureCanPreviewWebsiteAsync(eventId, cancellationToken);

        var pages = await sender.Send(
            new GetEventPagesByEventQuery(eventId, IncludeUnpublished: true),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventPagesByEventResponse>>.Success(
            pages,
            "Event preview pages retrieved successfully."));
    }

    [HttpPut("{eventId:guid}/pages/reorder")]
    public async Task<IActionResult> ReorderEventPages(
        Guid eventId,
        [FromBody] IReadOnlyList<Guid> pageIds,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ReorderEventPagesCommand(eventId, pageIds),
            cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Event pages reordered successfully."));
    }

    [HttpPut("{eventId:guid}/pages/{id:guid}/publish")]
    public async Task<IActionResult> PublishEventPage(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new PublishEventPageCommand(eventId, id), cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Event page published successfully."));
    }

    [HttpPut("{eventId:guid}/pages/{id:guid}/unpublish")]
    public async Task<IActionResult> UnpublishEventPage(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UnpublishEventPageCommand(eventId, id), cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Event page unpublished successfully."));
    }

    [HttpDelete("{eventId:guid}/pages/{id:guid}")]
    public async Task<IActionResult> DeleteEventPage(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteEventPageCommand(eventId, id), cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Event page deleted successfully."));
    }

    [HttpPut("{eventId:guid}/pages/{id:guid}")]
    public async Task<IActionResult> UpdateEventPage(Guid eventId, Guid id, UpdateEventPageRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateEventPageCommand(
            id,
            eventId,
            request.Name,
            request.Slug,
            request.PageType);

        await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Event page updated successfully."));
    }

    [HttpGet("{eventId:guid}/manage-pages/{id:guid}")]
    public async Task<IActionResult> GetManageEventPageById(
        Guid eventId,
        Guid id,
        CancellationToken cancellationToken)
    {
        await EnsureCanPreviewAsync(eventId, cancellationToken);

        var page = await sender.Send(
            new GetEventPageByIdQuery(eventId, id, IncludeUnpublished: true),
            cancellationToken);

        if (page is null)
        {
            throw new NotFoundException("Event page not found.");
        }

        return Ok(ApiResponse<GetEventPageByIdResponse>.Success(
            page,
            "Event page retrieved successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/pages/{id:guid}")]
    public async Task<IActionResult> GetEventPageById(Guid eventId, Guid id, CancellationToken cancellationToken)
    {
        var page = await sender.Send(new GetEventPageByIdQuery(eventId, id), cancellationToken);

        if (page is null)
        {
            throw new NotFoundException("Event page not found.");
        }

        return Ok(ApiResponse<GetEventPageByIdResponse>.Success(
            page,
            "Event page retrieved successfully."));
    }

    // Preview endpoints are limited to the roles that manage the event
    // (owner, organizer or event admin).
    private async Task EnsureCanPreviewAsync(Guid eventId, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanPreviewAsync(eventId, cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to preview this event.");
        }
    }

    // The website preview is limited to the organizer.
    private async Task EnsureCanPreviewWebsiteAsync(Guid eventId, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanPreviewWebsiteAsync(eventId, cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to preview this event.");
        }
    }
}
