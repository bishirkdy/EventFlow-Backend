using EventFlow.Api.Extensions;
using EventFlow.Registration.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventFlowApiDefaults("EventFlow Registration API");
builder.Services.AddRegistrationAuth(builder.Configuration);
builder.Services.AddRegistration(builder.Configuration);

var app = builder.Build();

app.UseEventFlowApiDefaults();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
