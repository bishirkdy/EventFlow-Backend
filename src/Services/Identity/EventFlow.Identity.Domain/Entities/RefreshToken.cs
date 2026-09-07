// Represents a refresh token issued to a user.
// Refresh tokens allow the client to obtain a new JWT
// access token without requiring the user to log in again.

using EventFlow.Identity.Domain.Common;

namespace EventFlow.Identity.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
    }

    public RefreshToken(
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    // Invalidate this refresh token.
    public void Revoke()
    {
        if (!IsRevoked)
        {
            RevokedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }
    }

    public User User { get; private set; } = null!;
}