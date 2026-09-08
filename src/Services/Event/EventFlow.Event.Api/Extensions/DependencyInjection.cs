using EventFlow.Event.Application.Common.Mappings;

namespace EventFlow.Event.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();
           
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventMappingProfile>();
        });
            return services;
        }
    }
}
