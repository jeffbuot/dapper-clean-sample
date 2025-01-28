using System.Data;
using DapperSample.Domain;

namespace DapperSample.Infrastructure;

/// <summary>
/// Our base repository interface
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IRepository<TEntity, TKey> where TEntity : IEntity
{
    /// <summary>
    /// Get entity based on id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TEntity> GetByIdAsync(TKey id);
    /// <summary>
    /// Get a list of entities based on filter or no filter.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetListAsync(IRequestFilter filter = default);
    /// <summary>
    /// Inserts entity to database and returns count of affected rows
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int?> InsertAsync(TEntity entity);  /// <summary>
    /// Updates entity to database and returns count of affected rows
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int?> UpdateAsync(TEntity entity);
    /// <summary>
    /// Deletes entity to database and returns count of affected rows
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int?> DeleteAsync(TKey id);
}

/// <summary>
/// Abstract base class for repositories...add more details here later
/// </summary>
/// <param name="unitOfWorkProvider"></param>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class Repository<TEntity, TKey>(ICurrentUnitOfWorkProvider unitOfWorkProvider,IUnitOfWork unitOfWork) :
    IRepository<TEntity, TKey> where TEntity : IEntity
{
    protected IDbConnection Connection => EnsureUnitOfWork().Connection;

    protected IDbTransaction Transaction => EnsureUnitOfWork().Transaction;

    // Automatically start UoW if repository is used without starting one
    private IUnitOfWork EnsureUnitOfWork()
    {
        if (unitOfWorkProvider.Current != null) return unitOfWorkProvider.Current;
        unitOfWork.BeginAsync().GetAwaiter().GetResult();
        unitOfWorkProvider.Current = unitOfWork;
        return unitOfWorkProvider.Current;
    }

    public abstract Task<TEntity> GetByIdAsync(TKey id);
    public abstract Task<IEnumerable<TEntity>> GetListAsync(IRequestFilter filter = default);
    public abstract Task<int?> InsertAsync(TEntity entity);
    public abstract Task<int?> UpdateAsync(TEntity entity);
    public abstract Task<int?> DeleteAsync(TKey id);
}