using FluentValidation;
using System.Text.Json;
namespace EventFlow.Registration.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> log)
{
    public async Task InvokeAsync(HttpContext c)
    {
        try
        {
            await next(c);
        }
        catch (ValidationException e)
        {
            c.Response.StatusCode = 400;
            await Write(c, new
            {
                success = false,
                message = "Validation failed.",
                errors = e.Errors.Select(x => new
                {
                    x.PropertyName,
                    x.ErrorMessage
                }
            )
            }
    );
        }
        catch (Exception e)
        {
            log.LogError(e, "Registration service error");
            c.Response.StatusCode = 500;
            await Write(c, new
            {
                success = false,
                message = "An unexpected error occurred.",
                data = (object?)null
            }
        );
        }
    }
    static Task Write(HttpContext c, object x)
    {
        c.Response.ContentType = "application/json";
        return c.Response.WriteAsync(JsonSerializer.Serialize(x));
    }
}
