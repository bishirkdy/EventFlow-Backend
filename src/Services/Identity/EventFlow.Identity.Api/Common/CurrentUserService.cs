using EventFlow.Security.Authentication;
using System.Security.Claims;

namespace EventFlow.Identity.Api.Common
{
    public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        public Guid UserId
        {
            get
            {
                var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userId, out var id))
                    throw new UnauthorizedAccessException("User is not authenticated.");

                return id;
            }
        }
    }
}
