using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Security.Configuration;
using EventFlow.Identity.Infrastructure.Authorization;
using EventFlow.Identity.Infrastructure.Persistence;
using EventFlow.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventFlow.Identity.Infrastructure.Persistence.Repositories;
using EventFlow.Messaging;

namespace EventFlow.Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("IdentityDatabase"));
            });

            services.AddEventFlowMessaging(configuration);
            // Bind Jwt configuration to JwtOptions.
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUserEventRoleRepository, UserEventRoleRepository>();
            services.AddScoped<IPhotographerInvitationRepository, PhotographerInvitationRepository>();

            // Services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPermissionService, PermissionService>();


            return services;
        }
    }
}
