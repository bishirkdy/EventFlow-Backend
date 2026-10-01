using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Security.Authentication;

// Provides information about the currently authenticated user.
public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    // Checks whether the current user is authenticated.
    public bool IsAuthenticated => accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    // Gets the current user's ID from the JWT claims.
    public Guid UserId
    {
        get
        {
            // Try to get the user ID from the NameIdentifier claim.
            // If it doesn't exist, try the "sub" claim.
            var value =
                accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? accessor.HttpContext?.User.FindFirstValue("sub");

            // Make sure the claim contains a valid GUID.
            if (!Guid.TryParse(value, out var userId))
            {
                throw new UnauthorizedException(
                    "Authenticated user identifier is missing.");
            }

            // Return the authenticated user's ID.
            return userId;
        }
    }
}
