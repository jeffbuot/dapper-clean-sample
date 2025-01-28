using Dapper;

namespace DapperSample.Infrastructure;

public class DatabaseSeeder(UnitOfWorkManager unitOfWorkManager)
{

    public async Task InitializeDatabaseAsync()
    { 
        //SimpleCrud.SetDialect(Dapper.SimpleCrud.Dialect.MySQL);
        using var uow = unitOfWorkManager.Begin();
        var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Books (
                    Id CHAR(36) PRIMARY KEY,
                    Title VARCHAR(255) NOT NULL,
                    PublishDate DATETIME NOT NULL,
                    AuthorId CHAR(36) NOT NULL
                );
                
                CREATE TABLE IF NOT EXISTS Authors (
                    Id CHAR(36) PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL
                );";
        await uow.Connection.ExecuteAsync(createTableQuery, transaction: uow.Transaction);
        await uow.CommitAsync();
    }

    public async Task SeedAsync()
    {
   
    }
}