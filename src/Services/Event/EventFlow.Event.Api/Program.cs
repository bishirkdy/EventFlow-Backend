using EventFlow.Event.Api.Extensions;
using EventFlow.Event.Application;
using EventFlow.Event.Infrastructure;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

