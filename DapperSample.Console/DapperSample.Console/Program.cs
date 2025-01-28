using System.Data;
using DapperSample.Domain;
using DapperSample.Infrastructure;
using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

DotEnv.Load();
var connectionString = Environment.GetEnvironmentVariable("ConnectionString");

var serviceProvider = new ServiceCollection()
    .AddTransient<IDbConnection>(sp => new MySqlConnection(connectionString))
    .AddSingleton<DatabaseSeeder>()
    .AddSingleton<UnitOfWorkManager>()
    .AddScoped<IUnitOfWork, UnitOfWork>()
    .AddScoped(typeof(IRepository<>), typeof(DapperGenericRepository<>))
    .BuildServiceProvider();

var databaseSeeder = serviceProvider.GetService<DatabaseSeeder>();
await databaseSeeder.InitializeDatabaseAsync();

var unitOfWorkManager = serviceProvider.GetService<UnitOfWorkManager>();

// Create a scope for unit of work transaction
using (var uow = unitOfWorkManager!.Begin())
{
    var authorRepository = uow.GetRepository<Author>();
    // add author
    var a1 = await authorRepository.InsertAsync(new Author() { Name = "Jeff" });
    var a2 =await authorRepository.InsertAsync(new Author() { Name = "Paul" });

    var bookRespository = uow.GetRepository<Book>();
    await bookRespository.InsertAsync(new Book{Title = "The Hobbit",PublishDate = DateTime.Now,AuthorId = a1});
    await bookRespository.InsertAsync(new Book{Title = "How to become a Chef",PublishDate = DateTime.Now,AuthorId = a2});

    await uow.CommitAsync();
}

using (var uow = unitOfWorkManager!.Begin())
{
    var authorRepository = uow.GetRepository<Author>();
    var bookRespository = uow.GetRepository<Book>();

    Console.WriteLine("Authors:");
    foreach (var author in await authorRepository.GetAllAsync())
    {
        Console.WriteLine(author);
    }

    Console.WriteLine("Books:");
    foreach (var author in await bookRespository.GetAllAsync())
    {
        Console.WriteLine(author);
    }
}