namespace DataModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

       
public class Cheep 
{
    public int CheepId { get; set; } //primary key
    public string text { get; set; }
    public DateTime TimeStamp { get; set; }
    public Author Author { get; set; }
}

[PrimaryKey(nameof(Name), nameof(Email))]
public class Author
{
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep>? Cheeps { get; set; }
}

public class CheepDbContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public CheepDbContext(DbContextOptions<CheepDbContext> options) : base(options) {}
}
