using EventFlow.Api.Extensions;
using EventFlow.Event.Api.Extensions;
using EventFlow.Event.Api.Middleware;
using EventFlow.Event.Application;
using EventFlow.Event.Infrastructure;
using EventFlow.Event.Infrastructure.Persistence;
using EventFlow.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 110 * 1024 * 1024;
});

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices(builder.Configuration)
    .AddEventFlowApiDefaults("EventFlow Event API", configuration: builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddEventFlowMessaging(builder.Configuration);
builder.Services.AddEventFlowMessaging(builder.Configuration);

var app = builder.Build();

app.UseEventFlowApiDefaults();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<EventPermissionMiddleware>();

app.MapControllers();

app.ApplyMigrations<EventCoreDbContext>();

app.Run();

