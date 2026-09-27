using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.NavigationItem;
using EventFlow.Event.Api.Requests.NavigationItems;
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

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/navigation-items")]
    [Authorize]
    public class NavigationItemsController(ISender sender) : ControllerBase
    {
        // Create navigation item
        [HttpPost("{eventId:guid}")]
        public async Task<IActionResult> CreateNavigationItem(
            Guid eventId,
            CreateNavigationItemRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateNavigationItemCommand(
                eventId,
                request.Label,
                request.PageId);

            var itemId = await sender.Send(command, cancellationToken);

            return Ok(new ApiResponse<Guid>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation item created successfully.",
                Data = itemId
            });
        }

        // Get navigation items
        [AllowAnonymous]
        [HttpGet("{eventId:guid}")]
        public async Task<IActionResult> GetNavigationItems(
            Guid eventId,
            CancellationToken cancellationToken)
        {
            var items = await sender.Send(
                new GetNavigationItemsQuery(eventId),
                cancellationToken);

            return Ok(
                new ApiResponse<IReadOnlyList<GetNavigationItemsResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Navigation items retrieved successfully.",
                    Data = items
                });
        }

        // Update navigation item
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

            return Ok(new ApiResponse<object?>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation item updated successfully.",
                Data = null
            });
        }

        // Delete navigation item
        [HttpDelete("{eventId:guid}/{id:guid}")]
        public async Task<IActionResult> DeleteNavigationItem(
            Guid eventId,
            Guid id,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new DeleteNavigationItemCommand(id, eventId),
                cancellationToken);

            return Ok(new ApiResponse<object?>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation item deleted successfully.",
                Data = null
            });
        }

        // Reorder navigation items
        [HttpPut("{eventId:guid}/reorder")]
        public async Task<IActionResult> ReorderNavigationItems(
            Guid eventId,
            [FromBody] IReadOnlyList<Guid> itemIds,
            CancellationToken cancellationToken)
        {
            var command = new ReorderNavigationItemsCommand(
                eventId,
                itemIds);

            await sender.Send(command, cancellationToken);

            return Ok(new ApiResponse<object?>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation items reordered successfully.",
                Data = null
            });
        }

        // Set navigation item visibility
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

            return Ok(new ApiResponse<object?>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = isVisible
                    ? "Navigation item shown successfully."
                    : "Navigation item hidden successfully.",
                Data = null
            });
        }

        // Get navigation item by page id
        [AllowAnonymous]
        [HttpGet("page/{pageId:guid}")]
        public async Task<IActionResult> GetNavigationItemByPage(
            Guid pageId,
            CancellationToken cancellationToken)
        {
            var item = await sender.Send(
                new GetNavigationItemByPageQuery(pageId),
                cancellationToken);

            return Ok(new ApiResponse<GetNavigationItemByPageResponse?>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation item retrieved successfully.",
                Data = item
            });
        }
    }
}