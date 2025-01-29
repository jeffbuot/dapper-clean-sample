using System.Data;
using DapperSample.Domain;
using InterpolatedSql.Dapper;

namespace DapperSample.Infrastructure.Authors;

public class AuthorRepository(ICurrentUnitOfWorkProvider unitOfWorkProvider, IUnitOfWork unitOfWork)
    : Repository<Author, Guid>(
        unitOfWorkProvider, unitOfWork)
{
    public async override Task<Author> GetByIdAsync(Guid id)
    {
        var builder = Connection.SqlBuilder($"""
                                             select *
                                             from Authors
                                             where Id = '{id}'
                                             limit 1;
                                             """);

        return await builder.QueryFirstAsync<Author>(transaction:Transaction);
    }

    public override async Task<IEnumerable<Author>> GetListAsync(IRequestFilter filter = default)
    {
        if (filter is AuthorRequestFilterDto filterDto)
        {
        }

        return [];
    }


    public async override Task<int?> InsertAsync(Author entity)
    {
        var builder = Connection.SqlBuilder($"""
                                             insert into Authors (Id, Name)
                                             values ({entity.Id}, '{entity.Name}');
                                             """);
        return await builder.ExecuteAsync(transaction: Transaction);
    }

    public override Task<int?> UpdateAsync(Author entity)
    {
        throw new NotImplementedException();
    }

    public override Task<int?> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}

public class AuthorRequestFilterDto : IRequestFilter
{
    public string? Sorting { get; set; }
    public string? FilterText { get; set; }
    public int MaxResultCount { get; set; }
    public int SkipCount { get; set; }
}