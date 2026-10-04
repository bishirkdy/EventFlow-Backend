using EventFlow.Operations.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Operations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddOperationsApplication(this IServiceCollection services)
    {
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
