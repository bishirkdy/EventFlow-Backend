using EventFlow.Identity.Application.Commands.RegisterUser;
using EventFlow.Identity.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public sealed class AuthController(ISender sender) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto request, CancellationToken cancellationToken)
        {
            var command =  new RegisterUserCommand(request);
            var userId = await sender.Send(command, cancellationToken);
            return Ok(userId);
        }
    }
}
