using System.Data;
using Dapper;
using DapperSample.Domain;

namespace DapperSample.Infrastructure;

public class DapperGenericRepository<T>(IDbConnection db) : IRepository<T>
    where T : class, IEntity
{
    private readonly IDbConnection _db = db;

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _db.GetAsync<T>(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _db.GetListAsync<T>();
    }

    public async Task<Guid> InsertAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        await _db.InsertAsync<Guid,T>(entity);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        var rowsAffected = await _db.UpdateAsync(entity);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var rowsAffected = await _db.DeleteAsync<T>(id);
        return rowsAffected > 0;
    }
}