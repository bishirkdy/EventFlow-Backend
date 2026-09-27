var builder = WebApplication.CreateBuilder(args);

//Configure connection between backend and frondent
if (builder.Environment.IsProduction())
{
  builder.Configuration.AddJsonFile(
      "appsettings.Docker.json",
      optional: false,
      reloadOnChange: false);
}

builder.Services.AddCors(option => {
    option.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

//configure the reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("Frontend");
app.MapReverseProxy();

app.Run();
