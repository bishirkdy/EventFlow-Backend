using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.NavigationItem;
using EventFlow.Event.Api.Requests.NavigationItems;
using EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem;
using EventFlow.Event.Application.Features.NavigationItem.Commands.DeleteNavigationItem;
using EventFlow.Event.Application.Features.NavigationItem.Commands.NavigationItemVisibility;
using EventFlow.Event.Application.Features.NavigationItem.Commands.ReorderNavigationItems;
using EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem;
using EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/navigation-items")]
    [Authorize]
    public class NavigationItemsController(IMapper mapper , ISender sender) : ControllerBase
    {
        [HttpPost("{navigationMenuId:guid}/items")]
        public async Task<IActionResult> CreateNavigationItem(Guid navigationMenuId,CreateNavigationItemRequest request,CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<CreateNavigationItemCommand>(request) with
            {
                NavigationMenuId = navigationMenuId
            };

            // Send command
            var itemId = await sender.Send(command,cancellationToken);
            var response = new ApiResponse<Guid>
            {
                Success = true,
                Message = "Navigation item created successfully.",
                Data = itemId
            };

            return Ok(response);
        }

        //get navigation items by navigation menu id
        [HttpGet("{navigationMenuId:guid}/items")]
        public async Task<IActionResult> GetNavigationItems(Guid navigationMenuId, CancellationToken cancellationToken)
        {
            // Send query
            var items = await sender.Send(new GetNavigationItemsQuery(navigationMenuId),cancellationToken);

            var response =
                new ApiResponse<IReadOnlyList<GetNavigationItemsResponse>>
                {
                    Success = true,
                    Message = "Navigation items retrieved successfully.",
                    Data = items
                };

            return Ok(response);
        }

        //Update navigation item by id
        [HttpPut("{navigationMenuId:guid}/items/{id:guid}")]
        public async Task<IActionResult> UpdateNavigationItem(Guid navigationMenuId, Guid id,UpdateNavigationItemRequest request,CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<UpdateNavigationItemCommand>(request) with
            {
                NavigationMenuId = navigationMenuId,
                Id = id
            };

            // Send command
            await sender.Send(command, cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Navigation item updated successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Delete navigation item
        [HttpDelete("{navigationMenuId:guid}/items/{id:guid}")]
        public async Task<IActionResult> DeleteNavigationItem(Guid navigationMenuId,Guid id,CancellationToken cancellationToken)
        {
            // Send command
            await sender.Send(new DeleteNavigationItemCommand(navigationMenuId, id),cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Navigation item deleted successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Chenge the order of navigation items
        [HttpPut("{navigationMenuId:guid}/items/reorder")]
        public async Task<IActionResult> ReorderNavigationItems(Guid navigationMenuId,[FromBody] IReadOnlyList<Guid> itemIds,CancellationToken cancellationToken)
        {
            var command = new ReorderNavigationItemsCommand(navigationMenuId,itemIds);
            await sender.Send(command, cancellationToken);
            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Navigation items reordered successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Toggle to set validation item visibility
        [HttpPatch("{navigationMenuId:guid}/items/{id:guid}/visibility")]
        public async Task<IActionResult> SetNavigationItemVisibility(Guid navigationMenuId,Guid id,
    [FromBody] bool isVisible,CancellationToken cancellationToken)
        {
            var command = new SetNavigationItemVisibilityCommand(navigationMenuId,id,isVisible);
            await sender.Send(command, cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = isVisible
                    ? "Navigation item shown successfully."
                    : "Navigation item hidden successfully.",
                Data = null
            };

            return Ok(response);
        }
    }
}
