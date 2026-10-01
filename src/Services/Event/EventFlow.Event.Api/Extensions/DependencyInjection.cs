using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Mappings;
using EventFlow.Event.Application.Abstractions.Authorization;
using EventFlow.Security.Authentication;
using EventFlow.Event.Api.Services;

namespace EventFlow.Event.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddControllers();

        services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error =>
                        string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "The supplied value is invalid."
                            : error.ErrorMessage)
                    .Distinct()
                    .ToList();

                var response = ApiResponse<object?>.Fail(
                    errors,
                    "One or more validation errors occurred.",
                    System.Net.HttpStatusCode.BadRequest);

                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
            };
        });

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpClient<IAuthorizationService, AuthorizationService>(client =>
        {
            var baseUrl = configuration["Services:Identity:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException(
                    "Services:Identity:BaseUrl is not configured.");
            }

            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventMappingProfile>();
        });

        return services;
    }
}