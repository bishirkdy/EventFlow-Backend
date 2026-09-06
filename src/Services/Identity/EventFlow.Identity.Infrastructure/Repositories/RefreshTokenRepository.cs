using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _context;

        public RefreshTokenRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            RefreshToken refreshToken,
            CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(
                refreshToken,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(
                    x => x.Token == token,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}
