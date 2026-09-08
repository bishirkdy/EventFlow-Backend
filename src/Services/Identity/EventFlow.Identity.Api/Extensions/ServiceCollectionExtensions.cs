using EventFlow.Identity.Application;
using EventFlow.Identity.Infrastructure;

namespace EventFlow.Identity.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,IConfiguration configuration)
        {
            services
                .AddApplication()
                .AddInfrastructure(configuration);

            return services;
        }
    }
}
