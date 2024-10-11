using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.RazorPages.Tests;

using Microsoft.VisualStudio.TestPlatform.TestHost;

public class CheepRepositoryTests
{
    
    
    [Theory]
    [InlineData("Adrian","adho@itu.dk")]
    [InlineData("Johannes","johje@itu.dk")]
    public async void CanCreateAuthor(string author, string email)
    {
        // Arrange
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<CheepDbContext>().UseSqlite(connection);

        using var context = new CheepDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        
        DbInitializer.SeedDatabase(context);
        
        ICheepRepository repository = new CheepRepository(context);

        // Act
        Console.WriteLine("Creating author...");
        repository.CreateAuthor(author, email);
        
        
        //Assert
        
        
    }
}