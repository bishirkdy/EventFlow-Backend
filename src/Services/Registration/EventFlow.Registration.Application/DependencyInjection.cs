
using EventFlow.Registration.Application.Behaviors;
using EventFlow.Registration.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Registration.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRegistrationApplication(this IServiceCollection services)
        {
            services.AddScoped<IWaitlistPromotionService, WaitlistPromotionService>();
            services.AddScoped<ICertificateTemplateBackgroundResolver, CertificateTemplateBackgroundResolver>();

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

            // Register AutoMapper profiles.
            services.AddAutoMapper(
                cfg => { },
                typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
