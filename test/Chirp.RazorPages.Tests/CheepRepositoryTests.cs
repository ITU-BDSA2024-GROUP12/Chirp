using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Chirp.RazorPages.Tests;

public class CheepRepositoryTests
{

    private readonly DbContextOptionsBuilder<CheepDbContext> _builder;

    public CheepRepositoryTests(ITestOutputHelper testOutputHelper)
    {

        SqliteConnection connection = new SqliteConnection("Filename=:memory:");
        connection.OpenAsync();
        _builder = new DbContextOptionsBuilder<CheepDbContext>().UseSqlite(connection);
        
    }
    
    [Theory]
    [InlineData("Adrian","adho@itu.dk")]
    public async void CanCreateAuthor_ThrowsException(string author, string email)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);
        
        ICheepRepository repository = new CheepRepository(context);

        // Act & Assert
        Assert.Throws<Exception>(() =>  repository.CreateAuthor(author, email));
        
    }
    
    [Theory]
    [InlineData("Johannes","johje@itu.dk")]
    public async void CanCreateAuthor(string author, string email)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);
        
        ICheepRepository repository = new CheepRepository(context);

        // Act & Assert
        bool result = repository.CreateAuthor(author, email);
        
        Assert.True(result);
        
    }
}