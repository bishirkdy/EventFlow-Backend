

namespace EventFlow.Identity.Application.Abstractions.Services
{
    //Password hashing interface
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
