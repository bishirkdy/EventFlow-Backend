using EventFlow.Event.Api.Extensions;
using EventFlow.Event.Application;
using EventFlow.Event.Infrastructure;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
  builder.Configuration.AddJsonFile(
      "appsettings.Docker.json",
      optional: false,
      reloadOnChange: false);

  // Environment variables must come AFTER Docker JSON
  builder.Configuration.AddEnvironmentVariables();
}

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 110 * 1024 * 1024;
});
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices()
    .AddJwtAuthentication(builder.Configuration)
       .AddSwaggerDocumentation();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerDocumentation();

app.UseApiMiddleware();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

