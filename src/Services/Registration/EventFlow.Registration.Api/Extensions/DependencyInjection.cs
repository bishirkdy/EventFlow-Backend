using EventFlow.Registration.Api.Services;
using EventFlow.Registration.Application;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Infrastructure;


namespace EventFlow.Registration.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<
    EventFlow.Security.Authentication.ICurrentUserService, CurrentUserService>();
        services.AddScoped<EventFlow.Security.Authentication.ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());
        services.AddScoped<IEventRegistrationAccessService, EventRegistrationAccessService>();

        services.AddRegistrationApplication();
        services.AddRegistrationInfrastructure(configuration);



        services.AddHttpClient(
            "EventService",
            client =>
            {
                var baseUrl = configuration["Services:Event:BaseUrl"];

                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }
            });

        return services;
    }
}