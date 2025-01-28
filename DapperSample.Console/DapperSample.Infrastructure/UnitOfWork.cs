using System.Data;
using DapperSample.Domain;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace DapperSample.Infrastructure;

public class UnitOfWork(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, string connectionString)
    : IUnitOfWork, IDisposable
{
    private IDbConnection? _dbConnection;
    private IDbTransaction? _dbTransaction;
    private bool _disposed;
    private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider = currentUnitOfWorkProvider;

    public IDbConnection Connection => _dbConnection ??= new MySqlConnection(connectionString);
    public IDbTransaction Transaction => _dbTransaction ?? throw new InvalidOperationException("The transaction has not been initialized.");
    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        // Already inside of Uow so we just ignore
        if (_currentUnitOfWorkProvider.Current != null) return;

        // Begin new db connection
        _dbConnection = new MySqlConnection(connectionString);
        _dbConnection.Open();
        _dbTransaction = _dbConnection.BeginTransaction();
        // Set this as the current UoW for the provider
        _currentUnitOfWorkProvider.Current = this;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        // Just ignore if the current UoW is not this one, usually this is called when we manually initialize UoW for
        // a specific code maybe.
        if (_currentUnitOfWorkProvider.Current != this) return;
        
        // Commit changes and clean
        _dbTransaction?.Commit();
        _dbTransaction = null;
        _currentUnitOfWorkProvider.Current = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if(_currentUnitOfWorkProvider.Current != this) return;
        _dbTransaction?.Rollback();
        _dbTransaction = null;
        _currentUnitOfWorkProvider.Current = null;
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        _dbTransaction?.Dispose();
        _dbConnection?.Dispose();
        _disposed = true;
    }

}