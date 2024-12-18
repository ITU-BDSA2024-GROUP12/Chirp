using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
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
    [Required] //Runtime validation
    [MaxLength(160)]
    public required string Text { get; set; } //Required keyword from https://learn.microsoft.com/da-dk/ef/core/miscellaneous/nullable-reference-types Compile time validation  
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public Author? Author { get; set; } //Duplicate author reference, maybe remove??
}

public class Author
{
    [MaxLength(256)]
    public int AuthorId { get; set; } //primary key
    [Required]
    [MaxLength(39)]
    [RegularExpression(@"^[a-zA-Z0-9-]{1,39}$")] //backend validation
    public required string Name { get; set; } //Required keyword from https://learn.microsoft.com/da-dk/ef/core/miscellaneous/nullable-reference-types
    [MaxLength(254)] //https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
    public string? Email { get; set; } 
    public ICollection<Cheep>? Cheeps { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
    public ICollection<Following>? Followings { get; set; }
}

public class CheepMention
{
    public int Id { get; set; } //primary key
    public int CheepId { get; set; }
    public Cheep? Cheep { get; set; }
    [Required]
    [MaxLength(39)]
    public string MentionedUsername { get; set; } //the name of the Author
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

public class Following
{
    public int Id { get; set; } //primary key
    public int FollowId { get; set; }
    public int AuthorId { get; set; }
}


public class CheepDbContext : IdentityDbContext<ChirpUser>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<CheepMention> CheepMentions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    public DbSet<Following> Followings { get; set; }

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
