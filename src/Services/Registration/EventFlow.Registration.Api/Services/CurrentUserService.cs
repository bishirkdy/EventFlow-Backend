using System.Security.Claims;
using EventFlow.Security.Authentication;
namespace EventFlow.Registration.Api.Services;

public sealed class CurrentUserService(IHttpContextAccessor a) : ICurrentUserService
{
    public bool IsAuthenticated => a.HttpContext?.User?.Identity?.IsAuthenticated == true;
    public Guid UserId
    {
        get
        {
            var v = a.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? a.HttpContext?.User.FindFirstValue("sub");
            return Guid.TryParse(v, out var id) ? id : Guid.Empty;
        }
    }
}
