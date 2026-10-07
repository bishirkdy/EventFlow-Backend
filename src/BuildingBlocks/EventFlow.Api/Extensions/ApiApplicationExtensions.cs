using EventFlow.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace EventFlow.Api.Extensions;

public static class ApiApplicationExtensions
{
    public const string CorsPolicy = "EventFlowCors";

    public static IServiceCollection AddEventFlowApiDefaults(this IServiceCollection services,
        string title,
        string version = "v1",
        IConfiguration? configuration = null)
    {
        services.AddControllers();
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();

        var allowedOrigins = configuration?.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
                else
                {
                    policy
                        .WithOrigins(
                            "http://localhost:4200",
                            "https://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            });
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = configuration?.GetValue<int?>("RateLimit:PermitLimit") ?? 300,
                        Window = TimeSpan.FromMinutes(
                            configuration?.GetValue<int?>("RateLimit:WindowMinutes") ?? 1),
                        AutoReplenishment = true,
                        QueueLimit = 0
                    }));
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto |
                ForwardedHeaders.XForwardedHost;

            // Containers sit behind the gateway/proxy on an internal network.
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

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
        app.UseForwardedHeaders();

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseCors(CorsPolicy);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRateLimiter();

        app.MapHealthChecks("/health");
        return app;
    }

    public static void ApplyMigrations<TContext>(this WebApplication app)
        where TContext : Microsoft.EntityFrameworkCore.DbContext
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        db.Database.Migrate();
    }
}
