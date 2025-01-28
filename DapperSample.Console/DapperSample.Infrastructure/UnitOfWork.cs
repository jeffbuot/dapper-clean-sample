using System.Data;
using DapperSample.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DapperSample.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;
    private readonly IServiceProvider _serviceProvider;

    public IDbConnection Connection => _dbConnection;
    public IDbTransaction Transaction => _dbTransaction;

    public UnitOfWork(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _dbConnection = _serviceProvider.GetRequiredService<IDbConnection>();
        _dbConnection.Open();
        _dbTransaction = _dbConnection.BeginTransaction();
        
    }

    public IRepository<T> GetRepository<T>() where T : IEntity
    {
        return _serviceProvider.GetRequiredService<IRepository<T>>();
    }

    public async Task<int> CommitAsync()
    {
        try
        {
            _dbTransaction.Commit();
            return 1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _dbTransaction.Rollback();
            return 0;
        }
    }

    public void Dispose()
    {
        _dbTransaction?.Dispose();
        _dbConnection?.Dispose();
    }
}