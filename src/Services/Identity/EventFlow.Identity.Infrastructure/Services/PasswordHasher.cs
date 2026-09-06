using EventFlow.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Identity;

using AspNetPasswordHasher = Microsoft.AspNetCore.Identity.PasswordHasher<object>;

namespace EventFlow.Identity.Infrastructure.Services
{
    public class PasswordHasher() : IPasswordHasher
    {
        private readonly AspNetPasswordHasher _hasher = new();

        public string Hash(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        public bool Verify(string password, string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(null! , passwordHash, password);
            return result == PasswordVerificationResult.Success;
        }



    }
}
