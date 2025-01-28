using System.Data;
using Dapper;
using DapperSample.Domain;
using DapperSample.Infrastructure;
using DapperSample.Infrastructure.Authors;
using dotenv.net;
using InterpolatedSql.Dapper;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

DotEnv.Load();
var connectionString = Environment.GetEnvironmentVariable("ConnectionString");

var serviceProvider = new ServiceCollection()
    .AddSingleton<ICurrentUnitOfWorkProvider, CurrentUnitOfWorkProvider>()
    .AddScoped<IUnitOfWork>(sp=> new UnitOfWork(sp.GetRequiredService<ICurrentUnitOfWorkProvider>(),connectionString))
    .AddScoped(typeof(AuthorRepository))
    .BuildServiceProvider();

var repository = serviceProvider.GetRequiredService<AuthorRepository>();

var authorA = new Author { Id = Guid.NewGuid(), Name = "Abby" };

    Console.WriteLine("Inserting author...");
    var res = await repository.InsertAsync(authorA);
    Console.WriteLine("Insert rows affected "+res);

    var uowProvider = serviceProvider.GetRequiredService<ICurrentUnitOfWorkProvider>();
    await uowProvider.Current?.CommitAsync()!;


var fromDbAuther = await repository.GetByIdAsync(authorA.Id);
Console.WriteLine($"Name: {fromDbAuther.Name} | Id: {fromDbAuther.Id}");
// using (var connection = new MySqlConnection(connectionString))
// {
//     var authorA = new Author { Id = Guid.NewGuid(), Name = "Jeff" };
//     
//     var builder = connection.SqlBuilder($"""
//                                          insert into Authors (Id, Name)
//                                          values ({authorA.Id}, '{authorA.Name}');
//                                          """);
//
//     var result = await builder.ExecuteAsync();
//     Console.WriteLine($"Rows affected {result}");
//     
//     var authors = connection.Query<Author>("SELECT * FROM Authors").ToList();
//     
//     // This does not work
//     // var authors = await connection.GetListAsync<Author>();
//
//     Console.WriteLine($"Authors: {authors.ToList().Count}");
//     foreach (var author in authors)
//     {
//         Console.WriteLine($"Name: {author.Name} | Id: {author.Id}");
//     } 
//
//
// }

// var serviceProvider = new ServiceCollection()
//     .AddTransient<IDbConnection>(sp => new MySqlConnection(connectionString))
//     .AddSingleton<DatabaseSeeder>()
//     .AddSingleton<UnitOfWorkManager>()
//     .AddScoped<IUnitOfWork, UnitOfWork>()
//     .AddScoped(typeof(IRepository<>), typeof(DapperGenericRepository<>))
//     .BuildServiceProvider();
//
// var databaseSeeder = serviceProvider.GetService<DatabaseSeeder>();
// await databaseSeeder.InitializeDatabaseAsync();
//
// var unitOfWorkManager = serviceProvider.GetService<UnitOfWorkManager>();

// Create a scope for unit of work transaction
// using (var uow = unitOfWorkManager!.Begin())
// {
//     var authorRepository = uow.GetRepository<Author>();
//     // add author
//     var a1 = await authorRepository.InsertAsync(new Author() { Name = "Jeff" });
//     var a2 =await authorRepository.InsertAsync(new Author() { Name = "Paul" });
//
//     var bookRespository = uow.GetRepository<Book>();
//     await bookRespository.InsertAsync(new Book{Title = "The Hobbit",PublishDate = DateTime.Now,AuthorId = a1});
//     await bookRespository.InsertAsync(new Book{Title = "How to become a Chef",PublishDate = DateTime.Now,AuthorId = a2});
//
//     await uow.CommitAsync();
// }

// using (var uow = unitOfWorkManager!.Begin())
// {
//     var authorRepository = uow.GetRepository<Author>();
//     var bookRespository = uow.GetRepository<Book>();
//
//     Console.WriteLine("Authors:");
//     foreach (var author in await authorRepository.GetAllAsync())
//     {
//         Console.WriteLine(author);
//     }
//
//     Console.WriteLine("Books:");
//     foreach (var author in await bookRespository.GetAllAsync())
//     {
//         Console.WriteLine(author);
//     }
// }