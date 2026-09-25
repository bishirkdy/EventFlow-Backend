using EventFlow.Identity.Api.Common;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController(IUserRepository userRepository) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{userId:guid}/summary")]
    [ProducesResponseType(typeof(ApiResponse<UserSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ApiResponse<UserSummaryResponse>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "User not found.",
                Data = null
            });
        }

        var displayName = $"{user.FirstName} {user.LastName}".Trim();

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = user.UserName;

        return Ok(new ApiResponse<UserSummaryResponse>
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "User retrieved successfully.",
            Data = new UserSummaryResponse(
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                displayName)
        });
    }
}
