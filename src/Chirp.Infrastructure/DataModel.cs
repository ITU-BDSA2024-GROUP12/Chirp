using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;



public class Cheep 
{
    public int CheepId { get; set; } //primary key
    [Required] //Is this required when using inline Required keyword?
    [MaxLength(500)]
    public required string Text { get; set; } //Required keyword from https://learn.microsoft.com/da-dk/ef/core/miscellaneous/nullable-reference-types
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    [MaxLength(256)]
    public Author? Author { get; set; } //Duplicate author reference, maybe remove??
}

public class Author
{
    public int AuthorId { get; set; } //primary key
    [MaxLength(256)]
    public required string Name { get; set; } //Required keyword from https://learn.microsoft.com/da-dk/ef/core/miscellaneous/nullable-reference-types
    [MaxLength(256)]
    public string? Email { get; set; }
    public ICollection<Cheep>? Cheeps { get; set; }
}

public class CheepDbContext : IdentityDbContext<ChirpUser>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public CheepDbContext(DbContextOptions<CheepDbContext> options) : base(options) {}
    
    //taken from https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_07/Slides.html
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>()
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}
