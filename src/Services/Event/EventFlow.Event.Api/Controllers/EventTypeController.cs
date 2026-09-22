using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Application.EventTypes.Queries.GetActiveEventTypes;
using EventFlow.Event.Application.Features.EventType.Queries.GetActiveEventTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/event-type")]
[Authorize]
public sealed class EventTypeController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEventTypes(CancellationToken cancellationToken)
    {
        var eventTypes = await sender.Send(new GetActiveEventTypesQuery(),cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<GetEventTypeResponse>>
        {
            Success = true,
            Message = "Event types retrieved successfully.",
            Data = eventTypes
        });
    }
}