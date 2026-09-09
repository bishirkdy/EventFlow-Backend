using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.EventPages;
using EventFlow.Event.Application.Features.EventPages.Commands;
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
    }
}
