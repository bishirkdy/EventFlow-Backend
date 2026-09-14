using EventFlow.Identity.Application.Abstractions.Authorization;
using System.Security.Claims;

namespace EventFlow.Identity.Api.Common
{
    public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor): ICurrentUserService
    {
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
