using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Infrastructure.Persistence;
using EventFlow.Event.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Event.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Register postersql
            services.AddDbContext<EventCoreDbContext>(option =>
            {
                option.UseNpgsql(configuration.GetConnectionString("EventCoreDatabase"));
            });

            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventSettingsRepository, EventSettingsRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IEventFeatureRepository, EventFeatureRepository>();
            services.AddScoped<IVenueRepository, VenueRepository>();

            return services;
        }
    }
}
