using EventFlow.Identity.Api.Contracts.Authorization;
using EventFlow.Identity.Application.Abstractions.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers
{
    [ApiController]
    [Route("api/authorization/v1")]
    public sealed class AuthorizationController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public AuthorizationController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost("check-permission")]
        public async Task<IActionResult> CheckPermission(CheckPermissionRequest request, CancellationToken cancellationToken)
        {
            var hasPermission = await _permissionService.HasPermissionAsync(
                request.UserId,
                request.EventId,
                request.Permission,
                cancellationToken);

            return Ok(new
            {
                hasPermission
            });
        }
    }
}
