using EventFlow.Event.Application.Exceptions;
using FluentValidation;

namespace EventFlow.Event.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Continue request pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log exception
            _logger.LogError(ex, "An unhandled exception occurred.");

            // Handle exception centrally
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context,Exception exception)
    {
        // Set response content type
        context.Response.ContentType = "application/json";

        // Determine HTTP status code
        context.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        // Determine response message
        var message = exception switch
        {
            ValidationException => "One or more validation errors occurred.",
            NotFoundException =>exception.Message,
            ConflictException =>exception.Message,
            ArgumentException =>exception.Message,
            InvalidOperationException =>exception.Message,
            _ => "An unexpected error occurred."
        };

        // Create standard API response
        var response = new
        {
            success = false,
            message,
            data = (object?)null
        };

        // Write response as JSON
        await context.Response.WriteAsJsonAsync(response);
    }
}