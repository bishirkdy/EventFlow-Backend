using EventFlow.Identity.Domain.Entities;


namespace EventFlow.Identity.Application.Abstractions.Repositories
{
    //Refreash token handling interface
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
