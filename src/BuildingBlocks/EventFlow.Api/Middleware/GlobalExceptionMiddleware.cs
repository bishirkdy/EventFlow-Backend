using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using EventFlow.Contracts.Common;
using EventFlow.SharedKernel.Exceptions;
using FluentValidation;

namespace EventFlow.Api.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleAsync(context, exception);
        }
    }

    private static async Task HandleAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ValidationException or ArgumentException or InvalidOperationException => HttpStatusCode.BadRequest,
            NotFoundException or KeyNotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            UnauthorizedException or UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            TimeZoneNotFoundException or InvalidTimeZoneException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        var errors = exception is ValidationException validation
            ? validation.Errors
                .Select(x => string.IsNullOrWhiteSpace(x.PropertyName)
                    ? x.ErrorMessage
                    : $"{x.PropertyName}: {x.ErrorMessage}")
                .Distinct()
                .ToList()
            : [GetSafeMessage(exception, statusCode)];

        var message = exception is ValidationException
            ? "One or more validation errors occurred."
            : GetSafeMessage(exception, statusCode);

        var errorCode = statusCode switch
        {
            HttpStatusCode.BadRequest => ApiErrorCodes.BadRequest,
            HttpStatusCode.NotFound => ApiErrorCodes.NotFound,
            HttpStatusCode.Conflict => ApiErrorCodes.Conflict,
            HttpStatusCode.Unauthorized => ApiErrorCodes.Unauthorized,
            HttpStatusCode.Forbidden => ApiErrorCodes.Forbidden,
            _ => ApiErrorCodes.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            ApiResponse<object?>.Fail(errors, message, statusCode, errorCode));
    }

    private static string GetSafeMessage(Exception exception, HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;
    }
}
