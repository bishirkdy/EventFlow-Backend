using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Security.Authentication;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public bool IsAuthenticated => accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? accessor.HttpContext?.User.FindFirstValue("sub");

            if (!Guid.TryParse(value, out var userId))
            {
                throw new UnauthorizedException("Authenticated user identifier is missing.");
            }

            return userId;
        }
    }
}
