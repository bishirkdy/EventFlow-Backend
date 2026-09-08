using EventFlow.Identity.Api.Middleware;

namespace EventFlow.Identity.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UseIdentityMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
