using EventFlow.Api.Extensions;
using EventFlow.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEventFlowApiDefaults("EventFlow Operations API");
builder.Services.AddEventFlowJwtAuthentication(builder.Configuration);

var app = builder.Build();
app.UseEventFlowApiDefaults();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
