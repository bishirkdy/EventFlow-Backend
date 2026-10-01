using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace EventFlow.Api.Middleware;

//Middleware to take CorrelationId logging with each request
public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();

        if (!Guid.TryParse(correlationId, out _))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await next(context);
        }
    }
}
