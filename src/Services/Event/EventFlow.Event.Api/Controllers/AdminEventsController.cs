using AutoMapper;
using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Requests.Admin;
using EventFlow.Event.Api.Responses.Admin;
using EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents;
using EventFlow.Event.Application.Features.Admin.Queries.GetAdminEventStats;
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
