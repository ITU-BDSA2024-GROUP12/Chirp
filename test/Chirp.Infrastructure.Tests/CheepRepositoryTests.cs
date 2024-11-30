using System.Text;
using Chirp.Core;
using Chirp.Infrastructure;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

using System.Threading.Tasks;

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

    private CheepDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CheepDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CheepDbContext(options);
    }
    
    //CreateCheep
    [Fact]
    public async Task CreateCheepWhenAuthorExists()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext,new AuthorRepository(dbContext));
        var author = new Author { AuthorId = 1, Name = "John Testman", Email = "john@test.com" };
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        var authorDto = new AuthorDTO { Name = "John Testman", Email = "john@test.com" };

        // Act
        var result = repository.CreateCheep(authorDto, "Hello World!",  null,"2024-11-05 12:00:00");

        // Assert
        Assert.True(result);
        var cheepInDb = await dbContext.Cheeps.FirstOrDefaultAsync(c => c.Text == "Hello World!");
        Assert.NotNull(cheepInDb);
        Assert.Equal(author.AuthorId, cheepInDb.AuthorId);
    }
    
    [Fact]
    public async Task CannotCreateCheepOver160()
    {
        // Arrange
        var author = "test";
        var email = "test@test.com";

        var sb = new StringBuilder();
        for (var i = 0; i < 161; i++)
        {
            sb.Append('.');
        }

        var text = sb.ToString();
            
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);
        ICheepRepository repository = new CheepRepository(context,new AuthorRepository(context));
        
        AuthorDTO authorDto = new AuthorDTO()
        {
            Name = author,
            Email = email
        };
        // Act & Assert
        Assert.Throws<InvalidDataException>(() => repository.CreateCheep(authorDto, text, null,DateTime.Now.ToString()));
    }
    
    [Theory]
    [InlineData("John John","jojo@itu.dk","Hi everyone!, this is a test!","2024-10-22 13:27:18")]
    [InlineData("Adrian","adho@itu.dk","This is starting to look like something...","2024-10-22 13:34:53")]
    public async void CanCreateCheep(string author, string email, string text, string time)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);
        ICheepRepository repository = new CheepRepository(context,new AuthorRepository(context));
        
        AuthorDTO authorDto = new AuthorDTO()
        {
            Name = author,
            Email = email
        };
        
        // Act
        repository.CreateCheep(authorDto, text, null, time);
        
        
        //Assert
        List<CheepDTO> result = await repository.GetMessagesFromAuthor(author, 0);
        
        Assert.Equal(result.First().Text,text);
    }
    /*needs bugfix
    [Fact]
    public async Task GetAuthorByEmailNonExistingAuthor()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => repository.GetAuthorByEmail("nonexistent@test.com"));
    }*/
    
    
    
    
    
    //GetMessage
    [Fact]
    public async Task GetMessageShouldReturnMessagesPaged()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext,new AuthorRepository(dbContext));
        var author = new Author { Name = "John Testman" };
        dbContext.Authors.Add(author);

        dbContext.Cheeps.Add(new Cheep { Author = author, Text = "Hello World!", TimeStamp = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetMessages(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Hello World!", result[0].Text);
    }
    
    [Fact]
    public async Task GetMessageReturn32Cheeps()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        DbInitializer.SeedDatabase(dbContext);
        var repository = new CheepRepository(dbContext,new AuthorRepository(dbContext));
        
        // Act
        var result = await repository.GetMessages(1);
        // Assert
        Assert.Equal(32, result.Count);
    }
    
    //GetMessagesFromAuthor
    [Fact]
    public async Task ReadMessagesFromAuthorPaged()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext,new AuthorRepository(dbContext));
        var author = new Author { Name = "John Testman" };
        dbContext.Authors.Add(author);

        dbContext.Cheeps.Add(new Cheep { Author = author, Text = "Hello from John!", TimeStamp = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetMessagesFromAuthor("John Testman", 1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Hello from John!", result[0].Text);
    }
}