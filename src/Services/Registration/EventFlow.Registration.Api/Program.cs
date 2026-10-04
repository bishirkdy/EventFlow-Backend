using EventFlow.Api.Extensions;
using EventFlow.Contracts.Common;
using EventFlow.Registration.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventFlowApiDefaults("EventFlow Registration API");

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error =>
                string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "The supplied value is invalid."
                    : error.ErrorMessage)
            .Distinct()
            .ToList();

        var response = ApiResponse<object?>.Fail(
            errors,
            "One or more validation errors occurred.",
            System.Net.HttpStatusCode.BadRequest);

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
    };
});

builder.Services.AddRegistrationAuth(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddRegistration(builder.Configuration);

var app = builder.Build();

app.UseEventFlowApiDefaults();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
