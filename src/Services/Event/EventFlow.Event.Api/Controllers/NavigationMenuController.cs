using AutoMapper;
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
    public class NavigationMenuController(IMapper mapper , ISender sender) : ControllerBase
    {
        //Create navigation menu 
        [HttpPost("{eventId:guid}/navigation-menus")]
        public async Task<IActionResult> CreateNavigationMenu(Guid eventId, CreateNavigationMenuRequest request, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<CreateNavigationMenuCommand>(request) with
            {
                EventId = eventId
            };

            // Send command
            var menuId = await sender.Send(command,cancellationToken);

            var response = new ApiResponse<Guid>
            {
                Success = true,
                Message = "Navigation menu created successfully.",
                Data = menuId
            };

            return Ok(response);
        }

        //Fetch navigation menus of event
        [HttpGet("{eventId:guid}/navigation-menus")]
        public async Task<IActionResult> GetNavigationMenusByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            // Send query
            var menus = await sender.Send(new GetNavigationMenusByEventQuery(eventId),cancellationToken);

            var response =
                new ApiResponse<IReadOnlyList<GetNavigationMenusByEventResponse>>
                {
                    Success = true,
                    Message = "Navigation menus retrieved successfully.",
                    Data = menus
                };

            return Ok(response);
        }

        //Get specific navigation menu of event 
        [HttpGet("{eventId:guid}/navigation-menus/{id:guid}")]
        public async Task<IActionResult> GetNavigationMenuById(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            // Send query
            var menu = await sender.Send(new GetNavigationMenuByIdQuery(eventId, id),cancellationToken);

            var response = new ApiResponse<GetNavigationMenuByIdResponse>
            {
                Success = true,
                Message = "Navigation menu retrieved successfully.",
                Data = menu
            };

            return Ok(response);
        }

        //Update navigation menu by id
        [HttpPut("{eventId:guid}/navigation-menus/{id:guid}")]
        public async Task<IActionResult> UpdateNavigationMenu(Guid eventId,Guid id,UpdateNavigationMenuRequest request,CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<UpdateNavigationMenuCommand>(request) with
            {
                EventId = eventId,
                Id = id
            };

            // Send command
            await sender.Send(command, cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Navigation menu updated successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Delete navigation menu of event
        [HttpDelete("{eventId:guid}/navigation-menus/{id:guid}")]
        public async Task<IActionResult> DeleteNavigationMenu(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            // Send command
            await sender.Send(new DeleteNavigationMenuCommand(eventId, id),cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Navigation menu deleted successfully.",
                Data = null
            };

            return Ok(response);
        }
    }
}
