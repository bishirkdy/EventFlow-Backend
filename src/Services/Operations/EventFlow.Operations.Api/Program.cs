using EventFlow.Api.Extensions;
using EventFlow.Operations.Application;
using EventFlow.Operations.Infrastructure;
using EventFlow.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddEventFlowApiDefaults(
    "EventFlow Operations API");

builder.Services.AddEventFlowJwtAuthentication(
    builder.Configuration);

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddOperationsApplication();

builder.Services.AddOperationsInfrastructure(
    builder.Configuration);

var app = builder.Build();

app.UseEventFlowApiDefaults();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();