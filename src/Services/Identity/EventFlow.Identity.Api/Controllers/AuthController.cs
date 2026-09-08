using EventFlow.Identity.Api.Common;
using EventFlow.Identity.Api.Contracts.Authentication;
using EventFlow.Identity.Application.Commands.Login;
using EventFlow.Identity.Application.Commands.LogoutUser;
using EventFlow.Identity.Application.Commands.RegisterUser;
using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            // Purpose: Revoke the user's refresh token.
            await sender.Send(
                new LogoutUserCommand(request.RefreshToken),
                cancellationToken);

            return NoContent();
        }
    }
}

