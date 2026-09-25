using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Exceptions;
using FluentValidation;

namespace EventFlow.Identity.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (exception is not ValidationException
                and not ConflictException
                and not NotFoundException
                and not UnauthorizedException
                and not ForbiddenException)
            {
                _logger.LogError(exception, "Unhandled exception occurred.");
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            ConflictException => StatusCodes.Status409Conflict,
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var message = exception switch
        {
            ValidationException => "One or more validation errors occurred.",
            ConflictException => "Conflict.",
            NotFoundException => "Resource not found.",
            UnauthorizedException => "Unauthorized.",
            ForbiddenException => "Forbidden.",
            _ => "An unexpected error occurred."
        };

        var errors = exception is ValidationException validationException
            ? validationException.Errors
                .Select(error => string.IsNullOrWhiteSpace(error.PropertyName)
                    ? error.ErrorMessage
                    : $"{error.PropertyName}: {error.ErrorMessage}")
                .Distinct()
                .ToList()
            : new List<string> { exception is ValidationException ? message : GetSafeError(exception) };

        var response = ApiResponse<object?>.Fail(
            errors,
            message,
            (System.Net.HttpStatusCode)statusCode);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }

    private static string GetSafeError(Exception exception) => exception switch
    {
        ConflictException or NotFoundException or UnauthorizedException or ForbiddenException => exception.Message,
        _ => "An unexpected error occurred."
    };
}
