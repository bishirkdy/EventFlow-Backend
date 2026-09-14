using EventFlow.Event.Api.Mappings;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Authentication;
using EventFlow.Event.Application.Abstractions.Authorization;

namespace EventFlow.Event.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpClient<IAuthorizationService, AuthorizationService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7001/");
        });

            services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventMappingProfile>();
        });
            return services;
        }
    }
}
