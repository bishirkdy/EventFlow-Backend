using EventFlow.Api.Middleware;
using EventFlow.Event.Api.Middleware;

namespace EventFlow.Event.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
            return app;
        }
    }
}
