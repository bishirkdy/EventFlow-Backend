using EventFlow.Identity.Domain.Entities;
using EventFlow.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence;

public class IdentityDbContext : DbContext, IOutboxDbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> dbContextOptions) : base(dbContextOptions) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserEventRole> UserEventRoles => Set<UserEventRole>();
    public DbSet<EventInvitation> EventInvitations => Set<EventInvitation>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        modelBuilder.ApplyOutbox();
        base.OnModelCreating(modelBuilder);
        IdentitySeedData.Seed(modelBuilder);
    }
}
