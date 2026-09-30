using EventFlow.Api.Extensions;
using EventFlow.Event.Api.Extensions;
using EventFlow.Event.Api.Middleware;
using EventFlow.Event.Application;
using EventFlow.Event.Infrastructure;
using EventFlow.Messaging;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 110 * 1024 * 1024;
});
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices(builder.Configuration)
    .AddEventFlowApiDefaults("EventFlow Event API")
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddEventFlowMessaging(builder.Configuration);

var app = builder.Build();



// Configure the HTTP request pipeline.
app.UseEventFlowApiDefaults();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<EventPermissionMiddleware>();

app.MapControllers();

app.Run();

