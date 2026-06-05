using Microsoft.EntityFrameworkCore.Storage;

namespace Review_Guard.Application.Abstractions.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task ExecuteAsync(Func<Task> action, CancellationToken ct = default);

    Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
