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

namespace EventFlow.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    // Limits registration requests using the "auth" rate-limiting policy.
    [EnableRateLimiting("auth")]
    [HttpPost("register")]

    // Documents the possible 201 Created response for Swagger/OpenAPI.
    [ProducesResponseType(typeof(ApiResponse<RegisterUserResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(RegisterUserRequest request,CancellationToken cancellationToken)
    {
        // Convert the API request into an application command.
        var command = new RegisterUserCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        // Send the command through MediatR.
        var result = await sender.Send(command, cancellationToken);

        // Create a standard API response.
        var response = new ApiResponse<RegisterUserResponse>
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status201Created,
            Message = "User registered successfully",
            Data = result
        };

        // Return HTTP 201 Created with the response body.
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [EnableRateLimiting("auth")]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        // Convert the API request into a login command.
        var command = new LoginUserCommand(
            request.Email,
            request.Password);

        // Send the command to the application layer.
        var result = await sender.Send(command, cancellationToken);

        // Store the access and refresh tokens in HTTP cookies.
        Response.SetAuthCookies(result.AccessToken, result.RefreshToken);

        // Tokens are stored in cookies, so they don't need to be returned in the body.
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
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken)
    {
        // Read the refresh token from the HTTP cookie.
        var refreshToken = Request.Cookies["refreshToken"];

        // Make sure a refresh token was provided.
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(
                ApiResponse<object?>.Fail(
                    ["Refresh token is required."],
                    "Unauthorized.",
                    System.Net.HttpStatusCode.Unauthorized));
        }

        // Send the refresh-token command to the application layer.
        var result = await sender.Send(
            new RefreshAccessTokenCommand(refreshToken), cancellationToken);

        // Replace the old authentication cookies with the new tokens.
        Response.SetAuthCookies(result.AccessToken, result.RefreshToken);

        // Return a successful response.
        return Ok(
            ApiResponse<object?>.Success(null, "Token refreshed successfully."));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        // Read the refresh token from the cookie.
        var refreshToken = Request.Cookies["refreshToken"];

        // Revoke the refresh token if one exists.
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await sender.Send(new LogoutUserCommand(refreshToken), cancellationToken);
        }

        // Remove authentication cookies from the browser.
        Response.ClearAuthCookies();

        return Ok(
            ApiResponse<object?>.Success(null, "Logout successful."));
    }

    [Authorize]
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        // Send the profile query 
        var result = await sender.Send(new GetProfileQuery(), cancellationToken);

        var response = new ApiResponse<UserProfileResponse>
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Profile retrieved successfully.",
            Data = result
        };

        // Return the user's profile.
        return Ok(response);
    }
}
