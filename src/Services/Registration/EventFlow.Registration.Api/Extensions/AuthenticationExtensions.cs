using EventFlow.Security.Authentication;

namespace EventFlow.Registration.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddRegistrationAuth(this IServiceCollection services, IConfiguration configuration)
        => services.AddEventFlowJwtAuthentication(configuration);
}
