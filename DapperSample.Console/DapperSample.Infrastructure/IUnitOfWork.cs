using System.Collections.Concurrent;
using System.Data;
using DapperSample.Domain;

namespace DapperSample.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }

    Task BeginAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// This will manage the current active UoW and ensures that repositories
/// participates in the existing UoW instead of starting a new one.
/// </summary>
public interface ICurrentUnitOfWorkProvider
{
    IUnitOfWork? Current { get; set; }
}

public class CurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider
{
    private Guid? _guid;
    public Guid Id => _guid ??= Guid.NewGuid();
    private static readonly AsyncLocal<Guid?> _currentScopeId = new();
    private static readonly ConcurrentDictionary<Guid, IUnitOfWork> _currentUnitOfWorks = new();
    // private static readonly AsyncLocal<IUnitOfWork?> _currentUow = new();

    public IUnitOfWork? Current { get; set; }
}