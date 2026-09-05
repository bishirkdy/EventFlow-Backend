using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Repositories
{
    // Implements the user persistence operations defined by IUserRepository.
    public class UserRepository(IdentityDbContext context) : IUserRepository
    {
        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Email == email,cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(string userName,CancellationToken cancellationToken)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        }

        public async Task AddAsync(User user,CancellationToken cancellationToken)
        {
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user,CancellationToken cancellationToken)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
