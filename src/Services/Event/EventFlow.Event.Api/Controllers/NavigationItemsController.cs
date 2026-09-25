using EventFlow.Event.Api.Common.Models;
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
        [HttpPost("{navigationMenuId:guid}/items")]
        public async Task<IActionResult> CreateNavigationItem(
            Guid navigationMenuId,
            CreateNavigationItemRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateNavigationItemCommand(
                navigationMenuId,
                request.Label,
                request.Url,
                request.PageId,
                request.DisplayOrder,
                request.OpenInNewTab);

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
        [HttpGet("{navigationMenuId:guid}/items")]
        public async Task<IActionResult> GetNavigationItems(
            Guid navigationMenuId,
            CancellationToken cancellationToken)
        {
            var items = await sender.Send(
                new GetNavigationItemsQuery(navigationMenuId),
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
        [HttpPut("{navigationMenuId:guid}/items/{id:guid}")]
        public async Task<IActionResult> UpdateNavigationItem(
            Guid navigationMenuId,
            Guid id,
            UpdateNavigationItemRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateNavigationItemCommand(
                navigationMenuId,
                id,
                request.Label,
                request.Url,
                request.PageId,
                request.DisplayOrder,
                request.OpenInNewTab,
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
        [HttpDelete("{navigationMenuId:guid}/items/{id:guid}")]
        public async Task<IActionResult> DeleteNavigationItem(
            Guid navigationMenuId,
            Guid id,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new DeleteNavigationItemCommand(navigationMenuId, id),
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
        [HttpPut("{navigationMenuId:guid}/items/reorder")]
        public async Task<IActionResult> ReorderNavigationItems(
            Guid navigationMenuId,
            [FromBody] IReadOnlyList<Guid> itemIds,
            CancellationToken cancellationToken)
        {
            var command = new ReorderNavigationItemsCommand(
                navigationMenuId,
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
        [HttpPatch("{navigationMenuId:guid}/items/{id:guid}/visibility")]
        public async Task<IActionResult> SetNavigationItemVisibility(
            Guid navigationMenuId,
            Guid id,
            [FromBody] bool isVisible,
            CancellationToken cancellationToken)
        {
            var command = new SetNavigationItemVisibilityCommand(
                navigationMenuId,
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

        //Get navigation item by page id
        [AllowAnonymous]
        [HttpGet("page/{pageId:guid}")]
        public async Task<IActionResult> GetNavigationItemByPage(Guid pageId,CancellationToken cancellationToken)
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
