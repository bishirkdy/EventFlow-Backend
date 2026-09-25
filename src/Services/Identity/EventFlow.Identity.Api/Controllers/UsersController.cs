using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Users;
using EventFlow.Identity.Application.Features.Queries.GetUserSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{userId:guid}/summary")]
    [ProducesResponseType(typeof(ApiResponse<UserSummaryResponse>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary(Guid userId,CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserSummaryQuery(userId),cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(
            ApiResponse<UserSummaryResponse>.Success(result, "User retrieved successfully."));
    }
}