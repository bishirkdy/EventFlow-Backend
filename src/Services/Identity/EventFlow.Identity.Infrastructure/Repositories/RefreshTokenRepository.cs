using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository(IdentityDbContext context) : IRefreshTokenRepository
    {

        //Add refreash token repo
        public async Task AddAsync(RefreshToken refreshToken,CancellationToken cancellationToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken,cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        //Get refreshtoken 
        public async Task<RefreshToken?> GetByTokenAsync(string token,CancellationToken cancellationToken)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token,cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
