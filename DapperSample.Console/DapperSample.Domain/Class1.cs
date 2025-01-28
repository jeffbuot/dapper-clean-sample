namespace DapperSample.Domain;


public interface IEntity
{
    Guid Id { get; set; }
}

public class Book : IEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    
    public Guid AuthorId { get; set; }

    // public override string ToString()
    // {
    //     return $"[Book] Id: {Id}, Title: {Title}, PublishDate: {PublishDate}";
    // }
}

public class Author : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    // public override string ToString()
    // {
    //     return $"[Author] Id: {Id}, Name: {Name}]";
    // }
}