using EventFlow.Registration.Application.Abstractions.Persistence;

namespace EventFlow.Registration.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork(RegistrationDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}
