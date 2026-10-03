using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Infrastructure.Background;
using EventFlow.Operations.Infrastructure.Persistence;
using EventFlow.Operations.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Operations.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOperationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OperationsDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("OperationsDatabase"));
        });

        services.AddHttpClient();

        services.AddScoped<IRegistrationClient, RegistrationClient>();
        services.AddScoped<IEventAuthorizationClient, EventAuthorizationClient>();
        services.AddScoped<IEventScheduleClient, EventScheduleClient>();
        services.AddScoped<IIdentityClient, IdentityClient>();

        services.AddHostedService<NotificationWorker>();

        services.AddScoped<LoggingEmailSender>();

        return services;
    }
}