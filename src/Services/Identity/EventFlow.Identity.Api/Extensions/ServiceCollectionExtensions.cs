using EventFlow.Contracts.Common;
using EventFlow.Identity.Application;
using EventFlow.Identity.Infrastructure;

namespace EventFlow.Identity.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,IConfiguration configuration)
        {
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
            services
                .AddApplication()
                .AddInfrastructure(configuration);

            return services;
        }
    }
}
