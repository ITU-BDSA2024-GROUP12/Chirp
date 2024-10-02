namespace DataModel;


public class Cheep 
{
    public string text { get; set; }
    public DateTime TimeStamp { get; set; }
    public Author Author { get; set; }
}

public class Author
{
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollections<Cheep> Cheeps { get; set; }>
}