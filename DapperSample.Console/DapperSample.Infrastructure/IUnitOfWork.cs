using System.Data;
using DapperSample.Domain;

namespace DapperSample.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> GetRepository<T>() where T : IEntity;
    Task<int> CommitAsync();

    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
}