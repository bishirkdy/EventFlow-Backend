using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Infrastructure.Persistence;
using EventFlow.Registration.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventFlow.Registration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRegistrationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<RegistrationDbContext>(
            options =>
            {
                options.UseNpgsql(
                        configuration.GetConnectionString("RegistrationDatabase"));
                    options.EnableDetailedErrors();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IRegistrationFormRepository, RegistrationFormRepository>();

        return services;
    }
}
