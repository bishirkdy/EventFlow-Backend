using EventFlow.Security.Authentication;
using System.Security.Claims;


namespace EventFlow.Event.Api.Services
{
    public sealed class CurrentUserService : ICurrentUserService    
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        public Guid UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userId, out var id))
                {
                    throw new UnauthorizedAccessException("User ID not found.");
                }

                return id;
            }
        }
    }
}
