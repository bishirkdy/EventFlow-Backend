using EventFlow.Identity.Domain.Entities;

namespace EventFlow.Identity.Application.Abstractions.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
}
