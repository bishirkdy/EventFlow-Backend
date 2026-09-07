using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Repositories
{
    public sealed class RoleRepository(IdentityDbContext context) : IRoleRepository
    {
        //Take role using get by id 
        public async Task<Role?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default)
        {
            return await context.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
