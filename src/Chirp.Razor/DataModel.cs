namespace DataModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
    public ICollection<Cheep> Cheeps { get; set; }
}

public class CheepDbContext : DbContext
{
    public DbSet<Cheep> Cheep { get; set; }
    public DbSet<Author> Author { get; set; }

    public CheepDbContext(DbContextOptions<CheepDbContext> options) : base(options) {}
}
