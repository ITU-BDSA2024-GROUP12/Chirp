using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Chirp.RazorPages.Tests;

public class CheepRepositoryTests
{
    private readonly DbContextOptionsBuilder<CheepDbContext> _builder;
    private readonly CheepDbContext context;
    private readonly ICheepRepository repository;

    public CheepRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        SqliteConnection connection = new SqliteConnection("Filename=:memory:");
        connection.OpenAsync();
        _builder = new DbContextOptionsBuilder<CheepDbContext>().UseSqlite(connection);
        context = new CheepDbContext(_builder.Options);
        context.Database.EnsureCreatedAsync();
        DbInitializer.SeedDatabase(context);
        repository = new CheepRepository(context);
    }

    [Theory]
    [InlineData("Adrian", "adho@itu.dk")]
    public async void CanCreateAuthor_ThrowsException(string author, string email)
    {
        // Arrange
        

        // Act & Assert
        Assert.Throws<Exception>(() => repository.CreateAuthor(author, email));
    }

    [Theory]
    [InlineData("Johannes", "johje@itu.dk")]
    public async void CanCreateAuthor(string author, string email)
    {
        // Arrange
        

        // Act & Assert
        bool result = repository.CreateAuthor(author, email);

        Assert.True(result);
    }

    
    [Theory]
    [InlineData("John John","jojo@itu.dk","Hi everyone!, this is a test!","2024-10-22 13:27:18")]
    [InlineData("Adrian","adho@itu.dk","This is starting to look like something...","2024-10-22 13:34:53")]
    public async void CanCreateCheep(string author, string email, string text, string time)
    {
        // Arrange
        
        
        // Applies the schema to the database
        
        
        // Act 
        repository.CreateCheep(author,email,text,time);
        
        
        //Assert
        List<CheepDTO> result = repository.ReadMessagesFromAuthor(author,0).Result;
        
        Assert.Equal(result.Last().Text,text);
        
    }
}