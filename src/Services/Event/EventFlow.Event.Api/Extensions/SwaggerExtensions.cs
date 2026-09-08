using Microsoft.OpenApi;

namespace EventFlow.Event.Api.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EventFlow Event Core API",
                    Version = "v1",
                    Description = "Event management APIs"
                });

                // Add JWT Bearer authentication to Swagger.
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "EventFlow Event Core API",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token: Bearer {your_token}"
                });

                // Add the Authorize button and require JWT authentication.
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });

            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(
            this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Event Core API v1");
            });

            return app;
        }
    }
}
