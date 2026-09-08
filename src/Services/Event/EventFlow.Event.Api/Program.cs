using EventFlow.Event.Api.Extensions;
using EventFlow.Event.Application;
using EventFlow.Event.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

