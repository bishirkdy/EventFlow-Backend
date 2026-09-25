using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Exceptions;
using FluentValidation;

namespace EventFlow.Event.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(exception, "Unhandled exception occurred.");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            TimeZoneNotFoundException => StatusCodes.Status400BadRequest,
            InvalidTimeZoneException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var message = exception switch
        {
            ValidationException => "One or more validation errors occurred.",
            NotFoundException or ConflictException or KeyNotFoundException => exception.Message,
            UnauthorizedAccessException => exception.Message,
            TimeZoneNotFoundException or InvalidTimeZoneException => exception.Message,
            ArgumentException or InvalidOperationException => exception.Message,
            _ => "An unexpected error occurred."
        };

        var errors = exception is ValidationException validationException
            ? validationException.Errors
                .Select(error => string.IsNullOrWhiteSpace(error.PropertyName)
                    ? error.ErrorMessage
                    : $"{error.PropertyName}: {error.ErrorMessage}")
                .Distinct()
                .ToList()
            : new List<string> { message };

        var response = ApiResponse<object?>.Fail(
            errors,
            message,
            (System.Net.HttpStatusCode)statusCode);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
}
