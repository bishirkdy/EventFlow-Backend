using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.DTOs.Users;
using EventFlow.Identity.Application.Features.Queries.GetUserByEmail;
using EventFlow.Identity.Application.Features.Queries.GetUserEventRolesLookup;
using EventFlow.Identity.Application.Features.Queries.GetUserSummary;
using EventFlow.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    // Internal service lookup keeps UserId out of the organizer's staff-management UI.
    [InternalServiceOnly]
    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(
            new GetUserByEmailQuery(email),
            cancellationToken);

        return Ok(ApiResponse<UserSummaryResponse>.Success(
            user,
            "User retrieved successfully."));
    }

    [InternalServiceOnly]
    [HttpGet("{userId:guid}/event-roles")]
    public async Task<IActionResult> GetEventRoles(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var roles = await sender.Send(
            new GetUserEventRolesLookupQuery(userId),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<UserEventRoleLookupResponse>>.Success(
            roles,
            "User event roles retrieved successfully."));
    }

    [InternalServiceOnly]
    [HttpGet("{userId:guid}/summary")]
    public async Task<IActionResult> GetSummary(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetUserSummaryQuery(userId),
            cancellationToken);

        return Ok(ApiResponse<UserSummaryResponse>.Success(
            result,
            "User retrieved successfully."));
    }
}
