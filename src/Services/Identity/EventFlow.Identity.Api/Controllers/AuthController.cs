using EventFlow.Identity.Api.Common;
using EventFlow.Identity.Api.Contracts.Authentication;
using EventFlow.Identity.Application.Commands.Login;
using EventFlow.Identity.Application.Commands.LogoutUser;
using EventFlow.Identity.Application.Commands.RegisterUser;
using EventFlow.Identity.Application.DTOs.Authentication;
using EventFlow.Identity.Application.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController(ISender sender) : ControllerBase
    {

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterUserResponse>),StatusCodes.Status201Created)]
        public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(request.UserName,request.Email,request.Password,request.FirstName,request.LastName);
            var result = await sender.Send(command, cancellationToken);
            var response = new ApiResponse<RegisterUserResponse>
            {
                Success = true,
                Message = "User registered successfully",
                Data = result
            };
            return StatusCode(StatusCodes.Status201Created,response);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email,request.Password);
            var result = await sender.Send(command,cancellationToken);

            //Setup tokens into the cookies
            Response.SetAuthCookies(result.AccessToken,result.RefreshToken);

            return Ok(new ApiResponse<object?>
            {
                Success = true,
                Message = "Login successful",
                Data = null
            });
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

            return NoContent();
        }

        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetProfileQuery(), cancellationToken);

            var response = new ApiResponse<UserProfileResponse>
            {
                Success = true,
                Message = "Profile retrieved successfully.",
                Data = result
            };

            return Ok(response);
        }
    }
}

