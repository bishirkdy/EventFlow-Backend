using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.Authorization;
using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Security.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers
{
    [ApiController]
    // Allows only internal services to access this controller.
    [InternalServiceOnly]

    // Base route for authorization endpoints.
    [Route("api/authorization/v1")]
    public sealed class AuthorizationController : ControllerBase
    {
        // Service used to check user permissions.
        private readonly IPermissionService _permissionService;

        public AuthorizationController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost("check-permission")]
        public async Task<IActionResult> CheckPermission(
            CheckPermissionRequest request,
            CancellationToken cancellationToken)
        {
            // Check whether the user has the requested permission for the event.
            var hasPermission = await _permissionService.HasPermissionAsync(
                request.UserId,
                request.EventId,
                request.Permission,
                cancellationToken);

            // Return the permission result.
            return Ok(
                ApiResponse<bool>.Success(hasPermission, "Permission check completed successfully."));
        }
    }
}
