using EventFlow.Api.Middleware;
using EventFlow.Messaging.Correlation;
using Microsoft.AspNetCore.Http;

namespace EventFlow.Api.Services;

// Reads the correlation id that CorrelationIdMiddleware stored on the request so
// it can be attached to outgoing integration events.
public sealed class HttpCorrelationIdProvider(IHttpContextAccessor httpContextAccessor) : ICorrelationIdProvider
{
    public string? Get()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return null;
        }

        if (httpContext.Items.TryGetValue(CorrelationIdMiddleware.CorrelationIdKey, out var value) &&
            value is string correlationId &&
            !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId;
        }

        return httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault();
    }
}
