using EventFlow.Event.Application.Abstractions.Authentication;
using EventFlow.Event.Application.Abstractions.Authorization;
using EventAuthorizationService = EventFlow.Event.Application.Abstractions.Authorization.IAuthorizationService;
using Microsoft.AspNetCore.Authorization;

namespace EventFlow.Event.Api.Middleware;

public sealed class EventPermissionMiddleware(
    RequestDelegate next,
    ICurrentUserService currentUser,
    EventAuthorizationService permissionService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null ||
            context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        if (!TryGetEventId(context, out var eventId))
        {
            await next(context);
            return;
        }

        var permission = context.Request.Method.Equals(HttpMethods.Get, StringComparison.OrdinalIgnoreCase)
            ? "event.view"
            : "event.update";

        var allowed = await permissionService.HasPermissionAsync(
            currentUser.UserId,
            eventId,
            permission,
            context.RequestAborted);

        if (!allowed)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "You do not have permission to access this event."
            });
            return;
        }

        await next(context);
    }

    private static bool TryGetEventId(HttpContext context, out Guid eventId)
    {
        var value = context.Request.RouteValues.TryGetValue("eventId", out var routeValue)
            ? routeValue?.ToString()
            : context.Request.RouteValues.TryGetValue("id", out var idValue)
                ? idValue?.ToString()
                : null;

        return Guid.TryParse(value, out eventId);
    }
}
