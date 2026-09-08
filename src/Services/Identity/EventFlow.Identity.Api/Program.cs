
using EventFlow.Identity.Api.Extensions;

namespace EventFlow.Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);


            builder.Services.AddSwaggerDocumentation();

            var app = builder.Build();

            app.UseSwaggerDocumentation();
            app.UseIdentityMiddleware();
            app.Run();
        }
    }
}
