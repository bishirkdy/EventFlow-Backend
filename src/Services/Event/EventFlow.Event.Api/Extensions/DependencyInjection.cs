using EventFlow.Api.Extensions;
using EventFlow.Event.Api.Mappings;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Authorization;
using EventFlow.Security.Authentication;

namespace EventFlow.Event.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEventFlowValidation();

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEventPreviewAccess, EventPreviewAccess>();

        services.AddHttpClient<IAuthorizationService, AuthorizationService>(client =>
        {
            var baseUrl = configuration["Services:Identity:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException(
                    "Services:Identity:BaseUrl is not configured.");
            }

            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventMappingProfile>();
            cfg.AddProfile<AdminMappingProfile>();
        });

        return services;
    }
}
