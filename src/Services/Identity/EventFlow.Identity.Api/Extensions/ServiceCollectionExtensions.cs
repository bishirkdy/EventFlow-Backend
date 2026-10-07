using EventFlow.Contracts.Common;
using EventFlow.Identity.Application;
using EventFlow.Identity.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(options =>
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

                    return new BadRequestObjectResult(response);
                };
            });
            services
                .AddApplication()
                .AddInfrastructure(configuration);

            return services;
        }
    }
}
