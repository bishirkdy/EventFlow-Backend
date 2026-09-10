using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.EventPages;
using EventFlow.Event.Application.Features.EventPages.Commands.CreateEventPage;
using EventFlow.Event.Application.Features.EventPages.Commands.DeleteEventPage;
using EventFlow.Event.Application.Features.EventPages.Commands.PublishEventPage;
using EventFlow.Event.Application.Features.EventPages.Commands.UnpublishEventPage;
using EventFlow.Event.Application.Features.EventPages.Queries.GetEventPagesByEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/event-page")]
    [Authorize]
    public class EventPageController(ISender sender, IMapper mapper) : ControllerBase
    {
        //Create event page
        [HttpPost("{eventId:guid}/pages")]
        public async Task<IActionResult> CreateEventPage(Guid eventId,CreateEventPageRequest request,CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<CreateEventPageCommand>(request) with
            {
                EventId = eventId
            };

            // Send command
            var pageId = await sender.Send(command,cancellationToken);

            var response = new ApiResponse<Guid>
            {
                Success = true,
                Message = "Event page created successfully.",
                Data = pageId
            };

            return Ok(response);
        }

        //Page of event
        [HttpGet("{eventId:guid}/pages")]
        public async Task<IActionResult> GetEventPagesByEvent(Guid eventId,CancellationToken cancellationToken)
        {
            // Send query
            var pages = await sender.Send(new GetEventPagesByEventQuery(eventId),cancellationToken);

            var response =
                new ApiResponse<IReadOnlyList<GetEventPagesByEventResponse>>
                {
                    Success = true,
                    Message = "Event pages retrieved successfully.",
                    Data = pages
                };

            return Ok(response);
        }

        //Publish event page
        [HttpPut("{eventId:guid}/pages/{id:guid}/publish")]
        public async Task<IActionResult> PublishEventPage(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            await sender.Send(new PublishEventPageCommand(eventId, id),cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Event page published successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Unpublish event page
        [HttpPut("{eventId:guid}/pages/{id:guid}/unpublish")]
        public async Task<IActionResult> UnpublishEventPage(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            await sender.Send(
                new UnpublishEventPageCommand(eventId, id), cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Event page unpublished successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Delete event page
        [HttpDelete("{eventId:guid}/pages/{id:guid}")]
        public async Task<IActionResult> DeleteEventPage(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            // Send command
            await sender.Send(new DeleteEventPageCommand(eventId, id), cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Event page deleted successfully.",
                Data = null
            };

            return Ok(response);
        }
    }
}
