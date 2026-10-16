using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.Request.Admin;
using EventFlow.Identity.Application.DTOs.Admin;
using EventFlow.Identity.Application.Features.Users.Queries.GetAdminUsers;
using EventFlow.Identity.Application.Features.Users.Queries.GetAdminUserStats;
using EventFlow.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

// Read-only user oversight endpoints for platform administrators.
[Authorize(Policy = PlatformAdminPolicy.Name)]
[ApiController]
[Route("api/v1/admin/users")]
public sealed class AdminUsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetAdminUsersRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAdminUsersQuery(request.Page, request.PageSize, request.Search),
            cancellationToken);

        return Ok(ApiResponse<PaginatedResponse<AdminUserListItemResponse>>.Success(
            result,
            "Users retrieved successfully."));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAdminUserStatsQuery(),
            cancellationToken);

        return Ok(ApiResponse<AdminUserStatsResponse>.Success(
            result,
            "User statistics retrieved successfully."));
    }
}
