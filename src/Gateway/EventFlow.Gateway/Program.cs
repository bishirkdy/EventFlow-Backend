var builder = WebApplication.CreateBuilder(args);

//Configure connection between backend and frondent
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
