using System.Net;
using AutoMapper;
using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Admin;
using EventFlow.Event.Api.Responses.Admin;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents;
using EventFlow.Event.Application.Features.Admin.Queries.GetAdminEventStats;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using EventFlow.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminEventListItemResponse = EventFlow.Event.Api.Responses.Admin.AdminEventListItemResponse;

namespace EventFlow.Event.Api.Controllers;

// Read-only event oversight endpoints for platform administrators.
[Authorize(Policy = PlatformAdminPolicy.Name)]
[ApiController]
[Route("api/v1/admin/events")]
public sealed class AdminEventsController(ISender sender, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEvents(
        [FromQuery] GetAdminEventsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAdminEventsQuery(request.Page, request.PageSize, request.Status, request.Search),
            cancellationToken);

        var response = new PaginatedResponse<AdminEventListItemResponse>
        {
            Items = mapper.Map<List<AdminEventListItemResponse>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        return Ok(ApiResponse<PaginatedResponse<AdminEventListItemResponse>>.Success(
            response,
            "Events retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEventByIdQuery(id), cancellationToken);

        if (result is null)
        {
            return NotFound(ApiResponse<GetEventResponse>.Fail(
                ["Event not found."],
                "Event not found.",
                HttpStatusCode.NotFound));
        }

        return Ok(ApiResponse<GetEventResponse>.Success(
            mapper.Map<GetEventResponse>(result),
            "Event retrieved successfully."));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAdminEventStatsQuery(), cancellationToken);

        return Ok(ApiResponse<AdminEventStatsResponse>.Success(
            new AdminEventStatsResponse
            {
                TotalEvents = result.TotalEvents,
                PublishedEvents = result.PublishedEvents,
                DraftEvents = result.DraftEvents,
                CancelledEvents = result.CancelledEvents
            },
            "Event statistics retrieved successfully."));
    }
}
