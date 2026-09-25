using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.NavigationMenu;
using EventFlow.Event.Application.Features.NavigationMenu.Commands.CreateNavigationMenu;
using EventFlow.Event.Application.Features.NavigationMenu.Commands.DeleteNavigationMenu;
using EventFlow.Event.Application.Features.NavigationMenu.Commands.UpdateNavigationMenu;
using EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenuById;
using EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenusByEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/navigation-menu")]
    [Authorize]
    public class NavigationMenuController(ISender sender) : ControllerBase
    {
        // Create navigation menu
        [HttpPost("{eventId:guid}/navigation-menus")]
        public async Task<IActionResult> CreateNavigationMenu(
            Guid eventId,
            CreateNavigationMenuRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateNavigationMenuCommand(
                eventId,
                request.Name,
                request.Location);

            var menuId = await sender.Send(command, cancellationToken);

            return Ok(new ApiResponse<Guid>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation menu created successfully.",
                Data = menuId
            });
        }

        // Fetch navigation menus of event
        [AllowAnonymous]
        [HttpGet("{eventId:guid}/navigation-menus")]
        public async Task<IActionResult> GetNavigationMenusByEvent(
            Guid eventId,
            CancellationToken cancellationToken)
        {
            var menus = await sender.Send(
                new GetNavigationMenusByEventQuery(eventId),
                cancellationToken);

            return Ok(
                new ApiResponse<IReadOnlyList<GetNavigationMenusByEventResponse>>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Navigation menus retrieved successfully.",
                    Data = menus
                });
        }

        // Get specific navigation menu
        [AllowAnonymous]
        [HttpGet("{eventId:guid}/navigation-menus/{id:guid}")]
        public async Task<IActionResult> GetNavigationMenuById(
            Guid eventId,
            Guid id,
            CancellationToken cancellationToken)
        {
            var menu = await sender.Send(
                new GetNavigationMenuByIdQuery(eventId, id),
                cancellationToken);

            return Ok(new ApiResponse<GetNavigationMenuByIdResponse>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation menu retrieved successfully.",
                Data = menu
            });
        }

        // Update navigation menu
        [HttpPut("{eventId:guid}/navigation-menus/{id:guid}")]
        public async Task<IActionResult> UpdateNavigationMenu(
            Guid eventId,
            Guid id,
            UpdateNavigationMenuRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateNavigationMenuCommand(
                eventId,
                id,
                request.Name,
                request.Location);

            await sender.Send(command, cancellationToken);

            return Ok(new ApiResponse<object?>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation menu updated successfully.",
                Data = null
            });
        }

        // Delete navigation menu
        [HttpDelete("{eventId:guid}/navigation-menus/{id:guid}")]
        public async Task<IActionResult> DeleteNavigationMenu(
            Guid eventId,
            Guid id,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new DeleteNavigationMenuCommand(eventId, id),
                cancellationToken);

            return Ok(new ApiResponse<object?>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Navigation menu deleted successfully.",
                Data = null
            });
        }
    }
}
