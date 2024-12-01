using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Infrastructure;

using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

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

    public ICollection<CheepMention>? Mentions { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
}

public class Author
{
    public int AuthorId { get; set; } //primary key
    [MaxLength(256)]
    public required string Name { get; set; } //Required keyword from https://learn.microsoft.com/da-dk/ef/core/miscellaneous/nullable-reference-types
    [MaxLength(256)]
    public string? Email { get; set; }
    public ICollection<Cheep>? Cheeps { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
}

public class CheepMention
{
    public int Id { get; set; } //primary key
    public int CheepId { get; set; }
    public Cheep? Cheep { get; set; }
    public string? MentionedUsername { get; set; } //the name of the Author
}

public class Notification
{
    public int Id { get; set; } //primary key
    public required int AuthorId { get; set; }
    public string? Content { get; set; }
    public int CheepId { get; set; }
    public Cheep? Cheep { get; set; }
    public DateTime TimeStamp { get; set; }
}


public class CheepDbContext : IdentityDbContext<ChirpUser>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<CheepMention> CheepMentions { get; set; }
    public DbSet<Notification> Notifications { get; set; }

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
