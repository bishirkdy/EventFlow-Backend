using EventFlow.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace EventFlow.Api.Extensions;

public static class ApiApplicationExtensions
{
    public static IServiceCollection AddEventFlowApiDefaults(this IServiceCollection services, string title, string version = "v1")
    {
        services.AddControllers();
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });

        return services;
    }

    public static WebApplication UseEventFlowApiDefaults(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapHealthChecks("/health");
        return app;
    }
}
