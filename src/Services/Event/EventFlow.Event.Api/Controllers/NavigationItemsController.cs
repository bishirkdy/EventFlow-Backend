using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.NavigationItem;
using EventFlow.Event.Api.Requests.NavigationItems;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Features.EventPages.Queries.GetEventIdByPage;
using EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem;
using EventFlow.Event.Application.Features.NavigationItem.Commands.DeleteNavigationItem;
using EventFlow.Event.Application.Features.NavigationItem.Commands.NavigationItemVisibility;
using EventFlow.Event.Application.Features.NavigationItem.Commands.ReorderNavigationItems;
using EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem;
using EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItemByPage;
using EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/navigation-items")]
[Authorize]
public class NavigationItemsController(ISender sender, IEventPreviewAccess previewAccess) : ControllerBase
{
    [HttpPost("{eventId:guid}")]
    public async Task<IActionResult> CreateNavigationItem(Guid eventId, CreateNavigationItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateNavigationItemCommand(
            eventId,
            request.Label,
            request.PageId);

        var itemId = await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<Guid>.Success(
            itemId,
            "Navigation item created successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}")]
    public async Task<IActionResult> GetNavigationItems(Guid eventId, CancellationToken cancellationToken)
    {
        var includeUnpublished = await previewAccess.CanPreviewAsync(eventId, cancellationToken);

        var items = await sender.Send(
            new GetNavigationItemsQuery(eventId, includeUnpublished),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetNavigationItemsResponse>>.Success(
            items,
            "Navigation items retrieved successfully."));
    }

    [HttpPut("{eventId:guid}/{id:guid}")]
    public async Task<IActionResult> UpdateNavigationItem(
        Guid eventId,
        Guid id,
        UpdateNavigationItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateNavigationItemCommand(
            id,
            eventId,
            request.Label,
            request.PageId,
            request.IsVisible);

        await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Navigation item updated successfully."));
    }

    [HttpDelete("{eventId:guid}/{id:guid}")]
    public async Task<IActionResult> DeleteNavigationItem(
        Guid eventId,
        Guid id,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteNavigationItemCommand(id, eventId),
            cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Navigation item deleted successfully."));
    }

    [HttpPut("{eventId:guid}/reorder")]
    public async Task<IActionResult> ReorderNavigationItems(Guid eventId, [FromBody] IReadOnlyList<Guid> itemIds, CancellationToken cancellationToken)
    {
        var command = new ReorderNavigationItemsCommand(eventId, itemIds);

        await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Navigation items reordered successfully."));
    }

    [HttpPatch("{eventId:guid}/{id:guid}/visibility")]
    public async Task<IActionResult> SetNavigationItemVisibility(
        Guid eventId,
        Guid id,
        [FromBody] bool isVisible,
        CancellationToken cancellationToken)
    {
        var command = new SetNavigationItemVisibilityCommand(
            eventId,
            id,
            isVisible);

        await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            isVisible ? "Navigation item shown successfully." : "Navigation item hidden successfully."));
    }

    [AllowAnonymous]
    [HttpGet("page/{pageId:guid}")]
    public async Task<IActionResult> GetNavigationItemByPage(Guid pageId, CancellationToken cancellationToken)
    {
        var eventId = await sender.Send(
            new GetEventIdByPageQuery(pageId),
            cancellationToken);

        var includeUnpublished = eventId.HasValue &&
            await previewAccess.CanPreviewAsync(eventId.Value, cancellationToken);

        var item = await sender.Send(
            new GetNavigationItemByPageQuery(pageId, includeUnpublished),
            cancellationToken);

        return Ok(ApiResponse<GetNavigationItemByPageResponse?>.Success(
            item,
            "Navigation item retrieved successfully."));
    }
}
