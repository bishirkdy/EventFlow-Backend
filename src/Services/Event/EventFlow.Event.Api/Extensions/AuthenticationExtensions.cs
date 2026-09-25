using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EventFlow.Event.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Prefer the HttpOnly cookie when present.
                            // Otherwise JwtBearer continues with the Authorization header.
                            var cookieToken = context.Request.Cookies["accessToken"];

                            if (!string.IsNullOrWhiteSpace(cookieToken))
                            {
                                context.Token = cookieToken;
                            }

                            return Task.CompletedTask;
                        },
                        OnChallenge = async context =>
                        {
                            if (context.Response.HasStarted) return;
                            context.HandleResponse();
                            var response = ApiResponse<object?>.Fail(
                                new[] { "Authentication is required." },
                                "Unauthorized.",
                                System.Net.HttpStatusCode.Unauthorized);
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsJsonAsync(response);
                        },
                        OnForbidden = async context =>
                        {
                            if (context.Response.HasStarted) return;
                            var response = ApiResponse<object?>.Fail(
                                new[] { "You do not have permission to access this resource." },
                                "Forbidden.",
                                System.Net.HttpStatusCode.Forbidden);
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsJsonAsync(response);
                        }
                    };
                });

            return services;
        }
    }
}
