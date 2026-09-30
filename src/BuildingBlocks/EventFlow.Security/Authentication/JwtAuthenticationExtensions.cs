using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using EventFlow.Contracts.Common;
using EventFlow.Security.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EventFlow.Security.Authentication;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddEventFlowJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Jwt:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Jwt:Audience is required.")
            .Validate(options => options.SecretKey.Length >= 32, "Jwt:SecretKey must contain at least 32 characters.")
            .ValidateOnStart();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                    ?? throw new InvalidOperationException("Jwt configuration is missing.");

                if (jwt.SecretKey.Length < 32)
                {
                    throw new InvalidOperationException("Jwt:SecretKey must contain at least 32 characters.");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (string.IsNullOrWhiteSpace(context.Token))
                        {
                            context.Token = context.Request.Cookies["accessToken"];
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        if (context.Response.HasStarted)
                        {
                            return;
                        }

                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<object?>.Fail(
                                ["Authentication is required."],
                                "Unauthorized.",
                                System.Net.HttpStatusCode.Unauthorized));
                    },
                    OnForbidden = async context =>
                    {
                        if (context.Response.HasStarted)
                        {
                            return;
                        }

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<object?>.Fail(
                                ["You do not have permission to access this resource."],
                                "Forbidden.",
                                System.Net.HttpStatusCode.Forbidden));
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}
