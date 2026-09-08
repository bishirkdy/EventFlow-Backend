
using EventFlow.Event.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Event.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR handlers.
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(
                    typeof(DependencyInjection).Assembly);

                config.AddOpenBehavior(
                    typeof(ValidationBehavior<,>));
            });

            // Register FluentValidation validators.
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);



            return services;
        }
    }
}
