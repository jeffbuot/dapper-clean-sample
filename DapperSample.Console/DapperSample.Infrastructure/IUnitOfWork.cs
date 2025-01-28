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
    private static readonly AsyncLocal<IUnitOfWork?> _currentUow = new();

    public IUnitOfWork? Current
    {
        get => _currentUow.Value;
        set => _currentUow.Value = value;
    }
}