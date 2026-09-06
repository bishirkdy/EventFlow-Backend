using EventFlow.Identity.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EventFlow.Identity.Api.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next,ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue the HTTP request pipeline.
                await _next(context);
            }
            catch (Exception exception)
            {
                // Handle any exception thrown by controllers,
                // MediatR handlers, repositories, etc.
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context,Exception exception)
        {
            // Log unexpected exceptions.
            // Do not expose internal exception details to clients.
            if (exception is not ValidationException
                && exception is not ConflictException
                && exception is not NotFoundException
                && exception is not UnauthorizedException
                && exception is not ForbiddenException)
            {
                _logger.LogError(exception,"Unhandled exception occurred.");
            }

            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                ConflictException => StatusCodes.Status409Conflict,
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            var title = exception switch
            {
                ValidationException => "Validation failed",
                ConflictException => "Conflict",
                NotFoundException => "Resource not found",
                UnauthorizedException => "Unauthorized",
                ForbiddenException => "Forbidden",
                _ => "An unexpected error occurred"
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = GetDetail(exception)
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }

        private static string GetDetail(Exception exception)
        {
            // FluentValidation contains useful validation
            // messages that can safely be returned to the client.
            if (exception is ValidationException)
            {
                return exception.Message;
            }


            // Application exceptions contain client-safe messages.
            if (exception is ConflictException
                or NotFoundException
                or UnauthorizedException
                or ForbiddenException)
            {
                return exception.Message;
            }

            // Never expose internal exception details in production.
            return "An unexpected error occurred.";
        }
    }
}
