using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.DTOs.UserEventRoles;
using EventFlow.Identity.Application.Features.EventTeam.Queries.GetMyEventPermissions;
using EventFlow.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/events/{eventId:guid}/my-permissions")]
public sealed class MyEventPermissionsController(ISender sender,ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid eventId,CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetMyEventPermissionsQuery(currentUser.UserId, eventId),
            cancellationToken);

        return Ok(ApiResponse<EventAccessResponse>.Success(
            result,
            "Event permissions retrieved successfully."));
    }
}
