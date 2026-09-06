using EventFlow.Identity.Domain.Entities;


namespace EventFlow.Identity.Application.Abstractions.Services
{
    // Defines the application's requirement for creating JWTs.
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
    }
}
