
using EventFlow.Registration.Application.Behaviors;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Registration.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRegistrationApplication(this IServiceCollection services)
        {
            services.AddScoped<IWaitlistPromotionService, WaitlistPromotionService>();

            services.AddMediatR(
                options =>
                {
                    options.RegisterServicesFromAssembly(
                        typeof(DependencyInjection).Assembly);

                    options.AddOpenBehavior(
                        typeof(ValidationBehavior<,>));
                  
                });

            services.AddValidatorsFromAssembly(
                typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
