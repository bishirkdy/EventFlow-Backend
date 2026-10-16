using EventFlow.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Api.Extensions;

public static class ApiValidationExtensions
{
    // Returns the standard ApiResponse envelope for model-state validation failures.
    public static IServiceCollection AddEventFlowValidation(this IServiceCollection services)
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

        return services;
    }
}
