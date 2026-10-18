using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Infrastructure.Authorization;
using EventFlow.Identity.Infrastructure.Persistence;
using EventFlow.Identity.Infrastructure.Persistence.Repositories;
using EventFlow.Identity.Infrastructure.Services;
using EventFlow.Messaging;
using EventFlow.Security.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("IdentityDatabase"));
        });

        services.AddEventFlowMessaging(configuration);
        services.AddOutbox<IdentityDbContext>(configuration);
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserEventRoleRepository, UserEventRoleRepository>();
        services.AddScoped<IEventInvitationRepository, EventInvitationRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}
