using EventFlow.Security.Authentication;

namespace EventFlow.Identity.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        => services.AddEventFlowJwtAuthentication(configuration);
}
