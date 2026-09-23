using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Infrastructure.Persistence;
using EventFlow.Event.Infrastructure.Repositories;
using EventFlow.Event.Infrastructure.Storage;
using EventFlow.Event.Infrastructure.Services.Identity;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.Infrastructure.Storage.Cloudinary;
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
            services.AddScoped<IEventPageRepository, EventPageRepository>();
            services.AddScoped<IPageSectionRepository, PageSectionRepository>();
            services.AddScoped<INavigationMenuRepository, NavigationMenuRepository>();
            services.AddScoped<INavigationItemRepository, NavigationItemRepository>();
            services.AddScoped<IEventTypeRepository, EventTypeRepository>();
            services.AddScoped<IFeatureRepository, FeatureRepository>();
            services.AddScoped<IEventTypeFeatureRepository,EventTypeFeatureRepository>();

            services.Configure<CloudinaryStorageOptions>(configuration.GetSection("Cloudinary"));
            services.AddSingleton<ICloudinaryStorage, CloudinaryStorage>();
            services.AddScoped<IFileStorage, CloudinaryFileStorage>();

            services.AddHttpClient<IUserDirectoryClient, UserDirectoryClient>(client =>
            {
                var identityBaseUrl = configuration["Services:Identity:BaseUrl"];

                if (string.IsNullOrWhiteSpace(identityBaseUrl))
                {
                    throw new InvalidOperationException(
                        "Identity service URL is not configured. Set Services:Identity:BaseUrl.");
                }

                client.BaseAddress = new Uri(identityBaseUrl);
            });

            return services;
        }
    }
}
