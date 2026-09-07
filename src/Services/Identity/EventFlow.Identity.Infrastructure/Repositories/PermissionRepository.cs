using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Repositories
{
    public sealed class PermissionRepository(IdentityDbContext context) : IPermissionRepository
    {

        // Retrieves a permission by its ID.
        public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Permissions.FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        }
    }
}
