using EventFlow.Api.Extensions;
using EventFlow.Identity.Api.Common;
using EventFlow.Identity.Api.Extensions;
using Microsoft.AspNetCore.RateLimiting;

namespace EventFlow.Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEventFlowApiDefaults("EventFlow Identity API");
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("auth", limiter =>
            {
                limiter.PermitLimit = 10;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueLimit = 0;
                limiter.AutoReplenishment = true;
            });
        });
        builder.Services.AddIdentityServices(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddScoped<
    EventFlow.Security.Authentication.ICurrentUserService,
    CurrentUserService>();
        builder.Services.AddScoped<EventFlow.Security.Authentication.ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());

        var app = builder.Build();

        app.UseEventFlowApiDefaults();
        app.UseRateLimiter();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
