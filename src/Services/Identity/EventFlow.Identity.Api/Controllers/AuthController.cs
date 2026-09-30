using EventFlow.Identity.Api.Common;
using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.Authentication;
using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using EventFlow.Identity.Application.Features.Commands.RegisterUser;
using EventFlow.Identity.Application.Features.Commands.LoginUser;
using EventFlow.Identity.Application.Features.Commands.LogoutUser;
using EventFlow.Identity.Application.Features.Commands.RefreshAccessToken;
using EventFlow.Identity.Application.Features.Queries.GetProfile;

namespace EventFlow.Identity.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController(ISender sender) : ControllerBase
    {

        [EnableRateLimiting("auth")]
    [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterUserResponse>),StatusCodes.Status201Created)]
        public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(request.UserName,request.Email,request.Password,request.FirstName,request.LastName);
            var result = await sender.Send(command, cancellationToken);
            var response = new ApiResponse<RegisterUserResponse>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status201Created,
                Message = "User registered successfully",
                Data = result
            };
            return StatusCode(StatusCodes.Status201Created,response);
        }

        [EnableRateLimiting("auth")]
    [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email,request.Password);
            var result = await sender.Send(command,cancellationToken);

            //Setup tokens into the cookies
            Response.SetAuthCookies(result.AccessToken,result.RefreshToken);

            return Ok(new ApiResponse<object?>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Login successful",
                Data = null
            });
        }

        [EnableRateLimiting("auth")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(ApiResponse<object?>.Fail(
                ["Refresh token is required."],
                "Unauthorized.",
                System.Net.HttpStatusCode.Unauthorized));
        }

        var result = await sender.Send(
            new RefreshAccessTokenCommand(refreshToken),
            cancellationToken);

        Response.SetAuthCookies(result.AccessToken, result.RefreshToken);

        return Ok(ApiResponse<object?>.Success(null, "Token refreshed successfully."));
    }

    [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            // Revoke the user's refresh token.
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await sender.Send(new LogoutUserCommand(refreshToken), cancellationToken);
            }
            Response.ClearAuthCookies();

            return Ok(ApiResponse<object?>.Success(null, "Logout successful."));
        }

        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetProfileQuery(), cancellationToken);

            var response = new ApiResponse<UserProfileResponse>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Profile retrieved successfully.",
                Data = result
            };

            return Ok(response);
        }
    }
}

