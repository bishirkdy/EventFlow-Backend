using EventFlow.Api.Extensions;
using EventFlow.Registration.Api.Extensions;
using EventFlow.Registration.Infrastructure.Persistence;
using EventFlow.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventFlowApiDefaults("EventFlow Registration API", configuration: builder.Configuration);
builder.Services.AddEventFlowValidation();

builder.Services.AddEventFlowJwtAuthentication(builder.Configuration);
builder.Services.AddRegistration(builder.Configuration);

var app = builder.Build();

app.UseEventFlowApiDefaults();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.ApplyMigrations<RegistrationDbContext>();

app.Run();
