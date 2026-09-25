using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Mappings;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Abstractions.Authentication;
using EventFlow.Event.Application.Abstractions.Authorization;

namespace EventFlow.Event.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
        services.AddControllers();
        services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
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
            client.BaseAddress = new Uri("https://localhost:7001/");
        });

            services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventMappingProfile>();
        });
            return services;
        }
    }
}
