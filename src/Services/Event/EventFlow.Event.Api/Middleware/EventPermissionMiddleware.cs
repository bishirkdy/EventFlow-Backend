using EventFlow.Security.Authentication;
using EventAuthorizationService =
    EventFlow.Event.Application.Abstractions.Authorization.IAuthorizationService;
using Microsoft.AspNetCore.Authorization;

namespace EventFlow.Event.Api.Middleware;

public sealed class EventPermissionMiddleware(RequestDelegate next, EventAuthorizationService permissionService)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null ||
            context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        // Claim ownership is authorized by the command handler,
        // which verifies that the authenticated user created the event.
        if (IsClaimOwnerRequest(context))
        {
            await next(context);
            return;
        }

        if (!TryGetEventId(context, out var eventId))
        {
            await next(context);
            return;
        }

        var requiredPermissions = ResolveRequiredPermissions(context.Request);

        var allowed = false;

        foreach (var permission in requiredPermissions)
        {
            if (await permissionService.HasPermissionAsync(
                    currentUserService.UserId,
                    eventId,
                    permission,
                    context.RequestAborted))
            {
                allowed = true;
                break;
            }
        }

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

    private static string[] ResolveRequiredPermissions(HttpRequest request)
    {
        var path = request.Path.Value ?? string.Empty;

        if (path.Contains("/feedback", StringComparison.OrdinalIgnoreCase))
        {
            return ["event.view"];
        }

        var isPhotoRequest = path.Contains(
            "/photos",
            StringComparison.OrdinalIgnoreCase);

        if (isPhotoRequest)
        {
            if (HttpMethods.IsGet(request.Method))
                return ["event.view", "photo.view"];

            if (HttpMethods.IsPost(request.Method))
                return ["photo.upload", "event.update"];

            if (HttpMethods.IsDelete(request.Method))
                return ["photo.manage", "event.update"];

            // Visibility moderation is organizer-only.
            if (HttpMethods.IsPatch(request.Method) || HttpMethods.IsPut(request.Method))
                return ["event.update"];
        }

        return HttpMethods.IsGet(request.Method)
            ? ["event.view"]
            : ["event.update"];
    }

    private static bool IsClaimOwnerRequest(HttpContext context)
    {
        return context.Request.Method.Equals(
                   HttpMethods.Post,
                   StringComparison.OrdinalIgnoreCase)
               && context.Request.Path.Value?.EndsWith(
                   "/claim-owner",
                   StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool TryGetEventId(
        HttpContext context,
        out Guid eventId)
    {
        var value =
            context.Request.RouteValues.TryGetValue(
                "eventId",
                out var routeValue)
                ? routeValue?.ToString()
                : context.Request.RouteValues.TryGetValue(
                    "id",
                    out var idValue)
                    ? idValue?.ToString()
                    : null;

        return Guid.TryParse(value, out eventId);
    }
}