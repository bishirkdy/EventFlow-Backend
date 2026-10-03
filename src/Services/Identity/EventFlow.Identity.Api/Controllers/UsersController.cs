using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Users;
using EventFlow.Identity.Application.Features.Queries.GetUserSummary;
using EventFlow.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController(
    ISender sender,
    IUserRepository users,
    IUserEventRoleRepository userEventRoles) : ControllerBase
{
    // Internal service lookup keeps UserId out of the organizer's staff-management UI.
    [InternalServiceOnly]
    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(email.Trim(), cancellationToken);
        if (user is null)
            return NotFound();

        return Ok(ApiResponse<UserSummaryResponse>.Success(
            new UserSummaryResponse(
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                $"{user.FirstName} {user.LastName}".Trim()),
            "User retrieved successfully."));
    }

    [InternalServiceOnly]
    [HttpGet("{userId:guid}/event-roles")]
    public async Task<IActionResult> GetEventRoles(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var roles = await userEventRoles.GetByUserAsync(
            userId,
            cancellationToken);

        var result = roles
            .Where(x => x.Role.Name is "Owner" or "Organizer" or "EventAdmin")
            .GroupBy(x => x.EventId)
            .Select(group => new UserEventRoleLookupResponse(
                group.Key,
                group.Select(x => x.Role.Name).Distinct().ToArray()))
            .ToList();

        return Ok(ApiResponse<IReadOnlyList<UserEventRoleLookupResponse>>.Success(
            result,
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

        if (result is null)
            return NotFound();

        return Ok(ApiResponse<UserSummaryResponse>.Success(
            result,
            "User retrieved successfully."));
    }
}

public sealed record UserEventRoleLookupResponse(
    Guid EventId,
    IReadOnlyList<string> RoleNames);
